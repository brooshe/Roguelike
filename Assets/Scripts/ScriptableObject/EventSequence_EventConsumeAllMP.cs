using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventSequence/EventConsumeAllMP")]
    public class EventSequence_EventConsumeAllMP : EventSequence_Event
    {
        public override bool CheckAndExecute(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            if (pawn.RemainMovePoint <= 0)
            {
                UIManager.Instance.Message("Not enough Move-Point!");
                return false;
            }

            bool result = base.CheckAndExecute(pawn, actor);
            //consume all move-points
            pawn.ConsumeMovePoint(pawn.RemainMovePoint);

            return result;
        }
    }
}