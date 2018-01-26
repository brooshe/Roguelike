using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager _instance;

    private Transform uiPanel;
    private Text useHint;
    private Text movePointArr;
	private Text movePoint;
	private Text messageBox;
    public ScrollRect QuestLogScroll;
    public Text QuestLogText;
    public float scroll;

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

        movePointArr = uiPanel.GetChild(1).GetComponent<Text>();
        movePoint = uiPanel.GetChild(2).GetComponent<Text>();
		messageBox = uiPanel.GetChild (3).GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () {

    }

	void Update()
	{
		if(messageBox.isActiveAndEnabled)
		{
			if ((msgTimeRemain -= Time.deltaTime) <= 0) 
			{
				messageBox.enabled = false;
			}
		}
        if(bUpdateQuest)
            QuestLogScroll.verticalScrollbar.value = 0;  
    }
	
	public void ShowUseButton(bool bShow)
    {
        if(useHint.isActiveAndEnabled != bShow)
            useHint.enabled = bShow;
    }

    public void SetMovePointLevel(uint[] arrMovePoint, int index)
    {
        string str = "行动力等级:";
        for(int i = 0; i < arrMovePoint.Length; ++i)
        {
            if (i != index)
                str = string.Concat(str, arrMovePoint[i]);
            else
                str = string.Format("{0}<color=green>{1}</color>", str, arrMovePoint[i]);
        }
        movePointArr.text = str;
    }
    public void SetCurMovePoint(uint movepoint)
	{
		movePoint.text = string.Format ("剩余行动力:{0}", movepoint);
	}

	public void Message(string msg)
	{
		if (!messageBox.isActiveAndEnabled)
			messageBox.enabled = true;
		
		messageBox.text = msg;
		msgTimeRemain = msgShowTime;
	}

    bool bUpdateQuest = false;
    public void QuestLog(string msg)
    {
        QuestLogText.text += msg + "\n";
        bUpdateQuest = true;
    }
}
