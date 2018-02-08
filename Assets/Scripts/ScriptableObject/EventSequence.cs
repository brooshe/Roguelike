using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventSequence")]
    public class EventSequence : ScriptableObject
    {
        public PlayerChecker Checker;
        public EventDefine Event;

        public bool CheckAndExecute(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            bool bPass = Checker == null || Checker.CheckPlayer(pawn, actor);
            if (bPass)
                Event.Execute(pawn, actor);

            return bPass;
        }
    }
}