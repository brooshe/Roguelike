using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ActorProperty
{
    [CreateAssetMenu(menuName = "PlayerCheck/Attribute")]
    public class PlayerAttributeChecker : PlayerChecker
    {
        public CharacterAtrribute Attribute;
        public int MinValue;
        public int MaxValue;


        public override bool CheckPlayer(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            int value = 0;
            switch(Attribute)
            {
                case CharacterAtrribute.STRENGTH:
                    value = pawn.CurStrength;
                    break;
                case CharacterAtrribute.MOVEPOINT:
                    value = pawn.CurMovePoint;
                    break;
                case CharacterAtrribute.INTEL:
                    value = pawn.CurIntel;
                    break;
                case CharacterAtrribute.SPIRIT:
                    value = pawn.CurSpirit;
                    break;
            }

            return value >= MinValue && value <= MaxValue;                
        }
    }
}