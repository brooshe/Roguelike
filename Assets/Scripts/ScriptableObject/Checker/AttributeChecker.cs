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

            Random.InitState(System.DateTime.Now.Second);
            int accum = 0;
            while(value-- > 0)
            {
                accum += Random.Range(0, 3);
            }

            return accum >= MinValue && accum <= MaxValue;                
        }
    }
}