using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventTimer/Simple")]
    public class EventTimer : ScriptableObject
    {
        public float DelayTime;

        public virtual bool Interupt(CharacterPawn pawn)
        {
            return false;
        }
        public virtual void Init(EventTimerInst inst)
        {
            inst.timeRemain = DelayTime;
        }
        public virtual void Clear(EventTimerInst inst)
        {

        }
    }
}