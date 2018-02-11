using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorMono
{
    [RequireComponent(typeof(Collider))]
    public class TriggerMono : MonoBehaviour
    {
        public Collider triggerCollider;

        public delegate void TriggerDelegate(Collider other);
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

        public void Enable(bool bEnable)
        {
            if(triggerCollider != null)
                triggerCollider.enabled = bEnable;
        }
        public void Show(bool bShow)
        {
            if (gameObject.activeSelf != bShow)
                gameObject.SetActive(bShow);
        }
    }
}
