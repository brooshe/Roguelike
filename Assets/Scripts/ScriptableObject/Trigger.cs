using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ActorMono;

namespace ActorProperty
{

    [CreateAssetMenu(menuName = "Trigger/Trigger")]
    public class Trigger : Actor
    {
        public PlayerChecker Checker;
        public List<EventDefine> EventList;

        public bool UseForOnce;

        public bool AutoTrigger;

        protected override void _OnLoad(ActorInstance.ActorBase actor)
        {
            base._OnLoad(actor);

            if(EventList != null)
            {
                foreach(var e in EventList)
                {
                    e.Init();
                }
            }
        }

        public virtual void OnEnter(CharacterPawn pawn, TriggerMono mono)
        {
        }
        public virtual void OnExit(CharacterPawn pawn, TriggerMono mono)
        {
        }

    }
    
}