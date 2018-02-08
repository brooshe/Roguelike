using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ActorMono;

namespace ActorProperty
{
    [CreateAssetMenu(menuName = "Trigger/Barrier")]
    public class Barrier : Trigger
    {
        public override void OnEnter(CharacterPawn pawn, TriggerMono mono)
        {
            BlockerMono blocker = mono as BlockerMono;            
            if (pawn != null && pawn.controller != null && pawn.PawnCollider != null && blocker != null && blocker.blockerCollider != null)
            {
                Physics.IgnoreCollision(pawn.PawnCollider, blocker.blockerCollider, true);                
            }
        }
        public override void OnExit(CharacterPawn pawn, TriggerMono mono)
        {
            BlockerMono blocker = mono as BlockerMono;            
            if (pawn != null && pawn.controller != null && pawn.PawnCollider != null && blocker != null && blocker.blockerCollider != null)
            {
                Physics.IgnoreCollision(pawn.PawnCollider, blocker.blockerCollider, false);
            }
        }

    }

}