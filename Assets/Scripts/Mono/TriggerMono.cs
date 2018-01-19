using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMono : MonoBehaviour {

	public delegate void TriggerDelegate (Collider other);
	public TriggerDelegate OnTrigger;

	void OnTriggerEnter(Collider other)
	{
		if (OnTrigger != null)
			OnTrigger (other);
	}
}
