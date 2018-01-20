using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class TriggerSocket
{
	[SerializeField]
	protected Trigger TriggerType;

    protected Trigger _curTrigger;
    public Trigger curTrigger
    {
        get { return _curTrigger; }
        set
        {
            if (value)
                value.socket = this;
            _curTrigger = value;
        }
    }

    public Vector3 LocalPosition;
    public Vector3 LocalEulerRotation;

	public void GenerateTrigger()
	{
		if (TriggerType != null) 
		{
            curTrigger = TriggerType.Clone<Trigger> ();
            curTrigger.Load ();
		}
	}
}
