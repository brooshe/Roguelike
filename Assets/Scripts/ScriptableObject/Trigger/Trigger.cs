using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ActorMono;

namespace Property
{

    [CreateAssetMenu(menuName = "Trigger/Trigger")]
    public class Trigger : Actor
    {
        public Checker Checker;
        public TargetFinder targetFinder;
        public bool UseForOnce;
        public bool AutoTrigger;
        public bool CanRemoteTrigger = false;
        public bool AutoEnable = true;

        public List<EventSequence> EntryEvents;
        public List<EventSequence> ExitEvents;

        protected override void _OnLoad(ActorInstance.ActorBase actor)
        {
            base._OnLoad(actor);
            ActorInstance.Trigger triggerInst = actor as ActorInstance.Trigger;
            if (triggerInst != null)
            {
                triggerInst.mono = triggerInst.actorTrans.GetComponent<TriggerMono>();
                
                if(!AutoEnable)
                    triggerInst.mono.Enable(false);

                if(CanRemoteTrigger)
                    GameLoader.Instance.RegisterRemoteTrigger(triggerInst);
            }
            else
                Debug.LogErrorFormat("Trigger {0} OnLoad an actor which is NOT a trigger instance!", name);

            if (EntryEvents != null)
            {
                foreach(var e in EntryEvents)
                {
                    e.Init();
                }
            }
            if (ExitEvents != null)
            {
                foreach (var e in ExitEvents)
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

        public virtual void FindTarget(PlayerController controller, ActorInstance.Trigger instance, ref List<CharacterPawn> targets)
        {
            targets.Clear();
            if (targetFinder == null)
                targets.Add(controller.Pawn);
            else
                targetFinder.FindTarget(controller, instance, ref targets);
        }

    }
    
}