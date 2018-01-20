using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager _instance;

    private Transform UIPanel;
    private Text UseHint;

    public static UIManager Instance
    { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }
	// Use this for initialization
	void Start () {
        UIPanel = transform.GetChild(0);
        UseHint = UIPanel.GetComponentInChildren<Text>();
        UseHint.enabled = false;
    }
	
	public void ShowUseButton(bool bShow)
    {
        if(UseHint.isActiveAndEnabled != bShow)
            UseHint.enabled = bShow;
    }
}
