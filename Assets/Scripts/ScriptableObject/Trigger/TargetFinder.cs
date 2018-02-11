using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{    
    public abstract class TargetFinder : ScriptableObject
    {
        public virtual void FindTarget(PlayerController controller, ActorInstance.Trigger instance, ref List<CharacterPawn> targets)
        {

        }
    }

}