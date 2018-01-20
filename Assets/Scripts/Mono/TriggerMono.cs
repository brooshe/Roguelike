using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerMono : MonoBehaviour {

	public delegate void TriggerDelegate (Collider other);
	public TriggerDelegate OnEnter;
    public TriggerDelegate OnExit;

    void OnTriggerEnter(Collider other)
	{
        Debug.Log("OnTriggerEnter");
		if (OnEnter != null)
            OnEnter(other);
	}

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (OnExit != null)
            OnExit(other);
    }
}
