using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager _instance;

    private Transform uiPanel;
    private Text useHint;
    private Text txtStrengthArr;
    private Text txtIntelArr;
    private Text txtSpiritArr;
    private Text txtMovePointArr;
	private Text movePoint;
	private Text messageBox;
    private Text roomName;
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

        Transform attrib = uiPanel.GetChild(1);
        txtStrengthArr = attrib.GetChild(0).GetComponent<Text>();
        txtIntelArr = attrib.GetChild(1).GetComponent<Text>();
        txtSpiritArr = attrib.GetChild(2).GetComponent<Text>();
        txtMovePointArr = attrib.GetChild(3).GetComponent<Text>();

        movePoint = uiPanel.GetChild(2).GetComponent<Text>();
		messageBox = uiPanel.GetChild (3).GetComponent<Text> ();
        roomName = uiPanel.GetChild(4).GetComponent<Text>();
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

    public void SetStrengthLevel(int[] array, int index)
    {
        SetLevel("力量等级:", txtStrengthArr, array, index);
    }
    public void SetIntelLevel(int[] array, int index)
    {
        SetLevel("知识等级:", txtIntelArr, array, index);
    }
    public void SetSpiritLevel(int[] array, int index)
    {
        SetLevel("神智等级:", txtSpiritArr, array, index);
    }
    public void SetMovePointLevel(int[] array, int index)
    {
        SetLevel("行动力等级:", txtMovePointArr, array, index);
    }
    private void SetLevel(string prefix, Text textComp, int[] array, int index)
    {        
        for (int i = 0; i < array.Length; ++i)
        {
            if (i != index)
                prefix = string.Concat(prefix, array[i]);
            else
                prefix = string.Format("{0}<color=green>{1}</color>", prefix, array[i]);
        }
        textComp.text = prefix;
    }
    public void SetCurMovePoint(int movepoint)
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

    public void SetRoomName(string name)
    {
        roomName.text = name;
    }
}
