using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "Check/Attribute")]
    public class AttributeChecker : Checker
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
                case CharacterAtrribute.AGILITY:
                    value = pawn.CurAgility;
                    break;
                case CharacterAtrribute.INTEL:
                    value = pawn.CurIntel;
                    break;
                case CharacterAtrribute.SPIRIT:
                    value = pawn.CurSpirit;
                    break;
            }

            int num = Dice.RandomDice(value);

            return num >= MinValue && num <= MaxValue;                
        }
    }
}