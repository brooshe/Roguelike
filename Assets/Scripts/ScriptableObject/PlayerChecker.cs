using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ActorProperty
{
    public abstract class PlayerChecker : ScriptableObject
    {
        public virtual bool CheckPlayer(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            return true;
        }
    }
}