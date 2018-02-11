using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Property
{
    public enum CharacterAtrribute
    {
        STRENGTH,
        AGILITY,
        INTEL,
        SPIRIT,
    }

    [CreateAssetMenu(menuName = "Config/Character")]
    public class CharacterDefine : ScriptableObject
    {
        public int[] StrengthArray;
        [FormerlySerializedAs("MovePointArray")]
        public int[] AgilityArray;
        public int[] IntelArray;
        public int[] SpiritArray;
        public int DefaultStrLev;
        [FormerlySerializedAs("DefaultMPLev")]
        public int DefaultAgiLev;
        public int DefaultIntLev;
        public int DefaultSprLev;
    }
}