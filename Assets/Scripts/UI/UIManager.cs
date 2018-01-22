using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager _instance;

    private Transform uiPanel;
    private Text useHint;
	private Text movePoint;
	private Text messageBox;

	public float msgShowTime = 2.0f;
	private float msgTimeRemain;

    public static UIManager Instance
    { get { return _instance; } }

    void Awake()
    {
        _instance = this;
		Init ();
    }

	void Init()
	{
		uiPanel = transform.GetChild(0);
		useHint = uiPanel.GetChild(0).GetComponent<Text>();
		useHint.enabled = false;

		movePoint = uiPanel.GetChild(1).GetComponent<Text>();
		messageBox = uiPanel.GetChild (2).GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () {

    }

	void Update()
	{
		return;
		if(messageBox.isActiveAndEnabled)
		{
			if ((msgTimeRemain -= Time.deltaTime) <= 0) 
			{
				messageBox.enabled = false;
			}
		}
	}
	
	public void ShowUseButton(bool bShow)
    {
        if(useHint.isActiveAndEnabled != bShow)
            useHint.enabled = bShow;
    }

	public void SetMovePoint(uint mobility)
	{
		movePoint.text = string.Format ("当前行动力:{0}", mobility);
	}

	public void Message(string msg)
	{
		if (!messageBox.isActiveAndEnabled)
			messageBox.enabled = true;
		
		messageBox.text = msg;
		msgTimeRemain = msgShowTime;
	}
}
