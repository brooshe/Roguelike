using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventSequence/Event")]
    public class EventSequence_Event : EventSequence
    {
        public string EventName;
        public string EventDesc;

        public override bool CheckAndExecute(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            if (pawn.RemainMovePoint <= 0)
            {
                UIManager.Instance.Message("Not enough Move-Point!");
                return false;
            }

            bool result = base.CheckAndExecute(pawn, actor);
            UIManager.Instance.QuestLog("你触发了" + EventName);
            UIManager.Instance.QuestLog(EventDesc);

            return result;
        }
    }
}