using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ActorProperty
{
    public enum CharacterAtrribute
    {
        STRENGTH,
        MOVEPOINT,
        INTEL,
        SPIRIT,
    }

    [CreateAssetMenu(menuName = "Config/Character")]
    public class CharacterDefine : ScriptableObject
    {
        public int[] StrengthArray;
        public int[] MovePointArray;
        public int[] IntelArray;
        public int[] SpiritArray;
        public int DefaultStrLev;
        public int DefaultMPLev;
        public int DefaultIntLev;
        public int DefaultSprLev;
    }
}