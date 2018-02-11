using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{    
    public abstract class TriggerFilter : ScriptableObject
    {
        public virtual bool Check(ActorInstance.Trigger trigger)
        {
            return true;
        }
    }

}