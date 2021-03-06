using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterPawn))]
public class PlayerController : MonoBehaviour
{
    private CharacterPawn m_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    private PlayerCamera playerCamera;

    public delegate void TriggerAction(PlayerController controller);
    public List<TriggerAction> TriggerActions = new List<TriggerAction>();

    public CharacterPawn Pawn { get { return m_Character; } }
        
    void Awake()
    {
        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<CharacterPawn>();
        m_Character.controller = this;
    }
    private void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
            playerCamera = m_Cam.gameObject.AddComponent<PlayerCamera>();
            playerCamera.Target = m_Character.transform;
            playerCamera.m_TargetOffset = new Vector3(0, 1.5f, 0);
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        GameLoader.Instance.EnterScene(m_Character);
        UIManager.Instance.MinimapSyncPawn(m_Character);
    }


    private void Update()
    {
        if (!m_Jump)
        {
            m_Jump = InputManager.GetButtonDown("Jump");
        }
        
        UIManager.Instance.ShowUseButton(TriggerActions.Count > 0);

#if !MOBILE_INPUT
        if(InputManager.GetButtonDown("Use"))
        {
            if (TriggerActions.Count > 0)
            {
                TriggerAction action = TriggerActions[0];
                Debug.Log("RemoveTriggerAction");
                TriggerActions.RemoveAt(0);
                action(this);
            }

        }        
#endif
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
        float h = InputManager.GetAxis("Horizontal");
        float v = InputManager.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v*m_CamForward + h*m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v*Vector3.forward + h*Vector3.right;
        }
#if !MOBILE_INPUT
		// walk speed multiplier
	    if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

        // pass all parameters to the character control script
        m_Character.Move(m_Move, crouch, m_Jump);
        m_Jump = false;
    }
}

