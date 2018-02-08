using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Property
{
    [System.Serializable]
    public class TriggerSocket
    {
        [SerializeField]
        protected Trigger TriggerType;

        public Vector3 LocalPosition;
        public Vector3 LocalEulerRotation;

        public virtual ActorInstance.Trigger GenerateTrigger()
        {
            ActorInstance.Trigger trigger = null;
            if (TriggerType != null)
            {
                trigger = new ActorInstance.Trigger(TriggerType);
                trigger.socket = this;         
            }
            return trigger;
        }
    }
}
