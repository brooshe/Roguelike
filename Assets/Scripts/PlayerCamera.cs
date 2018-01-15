using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private Transform m_Target;
    public Transform Target
    {
        set { m_Target = value; }
        get { return m_Target; }
    }

    [SerializeField]
    private float m_MoveSpeed = 10f;                      // How fast the rig will move to keep up with the target's position.
    [Range(0f, 10f)]
    [SerializeField]
    private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField]
    private float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField]
    private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
    [SerializeField]
    private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
    [SerializeField]
    private bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
    [SerializeField]
    private bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return
    
    public Vector3 m_TargetOffset = Vector3.zero;

    private float m_LookAngle;                    // The rig's y axis rotation.
    private float m_TiltAngle;                    // The pivot's x axis rotation.
    private const float k_LookDistance = 5f;    // How far in front of the pivot the character's look target is.
    private Vector3 m_PivotEulers;
    private Quaternion m_PivotTargetRot;
    private Quaternion m_TransformTargetRot;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        HandleRotationMovement();
    }

    private void FixedUpdate()
    {
        // we update from here if updatetype is set to Fixed, or in auto mode,
        // if the target has a rigidbody, and isn't kinematic.
        FollowTarget(Time.deltaTime);
    }

    protected void FollowTarget(float deltaTime)
    {
        if (m_Target == null) return;
        // Move the rig towards target position.
        Vector3 lookAtLocation = m_Target.position + m_TargetOffset;
        Vector3 desiredLocation = lookAtLocation + m_PivotTargetRot * Vector3.back * k_LookDistance;
        transform.position = Vector3.Lerp(transform.position, desiredLocation, deltaTime * m_MoveSpeed);

    }

    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
            return;

        // Read the user input
        var x = InputManager.GetAxis("Mouse X");
        var y = InputManager.GetAxis("Mouse Y");

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        m_LookAngle += x * m_TurnSpeed;

        // Rotate the rig (the root object) around Y axis only:
        m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

        if (m_VerticalAutoReturn)
        {
            // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
            // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
            // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
            m_TiltAngle = y > 0 ? Mathf.Lerp(0, -m_TiltMin, y) : Mathf.Lerp(0, m_TiltMax, -y);
        }
        else
        {
            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= y * m_TurnSpeed;
            // and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
        }

        // Tilt input around X is applied to the pivot (the child of this object)
        //m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);
        m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_LookAngle, 0);

        if (m_TurnSmoothing > 0)
        {
            //m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
        }
        else
        {
            //m_Pivot.localRotation = m_PivotTargetRot;
            //transform.localRotation = m_TransformTargetRot;
        }
        Vector3 lookAtLocation = m_Target.position + m_TargetOffset;
        transform.LookAt(lookAtLocation);
    }
}
