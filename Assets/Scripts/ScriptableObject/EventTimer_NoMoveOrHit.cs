using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventTimer/NoMoveOrHit")]
    public class EventTimer_NoMoveOrHit : EventTimer
    {
        public override bool Interupt(CharacterPawn pawn)
        {
            if (pawn == null || pawn.Velocity.sqrMagnitude > 0.001f)
            {
                Debug.LogWarning("Timer interrupt!");
                return true;
            }

            return false;
        }

        public override void Init(EventTimerInst inst)
        {
            base.Init(inst);
            inst.pawn.OnHit += inst.Stop;
            inst.pawn.OnTransport += inst.Stop;
        }
        public override void Clear(EventTimerInst inst)
        {
            inst.pawn.OnHit -= inst.Stop;
            inst.pawn.OnTransport -= inst.Stop;
        }
    }
}