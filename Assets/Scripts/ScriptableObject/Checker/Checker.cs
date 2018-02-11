using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    public abstract class Checker : ScriptableObject
    {
        public virtual bool CheckPlayer(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            return true;
        }
    }
}