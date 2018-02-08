using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ActorProperty
{
    [CreateAssetMenu(menuName = "PlayerCheck/MovePoint")]
    public class PlayerMovePointChecker : PlayerChecker
    {
        public int Value;

        public override bool CheckPlayer(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            return pawn.RemainMovePoint >= Value;         
        }
    }
}