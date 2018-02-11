using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "Check/MovePoint")]
    public class MovePointChecker : Checker
    {
        public int Value;

        public override bool CheckPlayer(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            return pawn.RemainMovePoint >= Value;         
        }
    }
}