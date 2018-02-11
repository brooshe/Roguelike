using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ActorMono;

namespace ActorInstance
{
    public class Trigger : ActorBase
    {
        
        public TriggerMono mono;
        public Property.TriggerSocket socket;
        
        public Room parentRoom;

        public Property.Trigger triggerProp
        {
            get { return property as Property.Trigger; }
        }
        //if use-for-once, those ones who trigger this must be recorded
        protected HashSet<CharacterPawn> activatePawns;
        protected List<CharacterPawn> targets;

        public Trigger(Property.Trigger prop) : base(prop)
        {            
            if (mono == null)
            {
                Debug.LogError("TriggerMono is null!");
            }
            else
            {
                mono.OnEnter = OnTriggerEnter;
                mono.OnExit = OnTriggerExit;
            }

            if (prop.UseForOnce)
                activatePawns = new HashSet<CharacterPawn>();
        }

        private void OnTriggerEnter(Collider other)
        {
            CharacterPawn pawn = other.GetComponent<CharacterPawn>();
            if (pawn != null && pawn.controller != null)
            {
                if (activatePawns != null && activatePawns.Contains(pawn))
                    return;

                if (triggerProp.AutoTrigger)
                    TriggerAction(pawn.controller);
                else
                {
                    Debug.Log("AddTriggerAction");
                    pawn.controller.TriggerActions.Add(TriggerAction);                    
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            CharacterPawn pawn = other.GetComponent<CharacterPawn>();
            if (pawn != null && pawn.controller != null)
            {
                if (triggerProp.AutoTrigger)
                    TriggerExitAction(pawn.controller);
                else
                {
                    Debug.Log("RemoveTriggerAction");
                    pawn.controller.TriggerActions.Remove(TriggerAction);
                }
            }
        }

        private void TriggerAction(PlayerController controller)
        {
            if (CheckAvailable(controller.Pawn))
            {
                OnTriggerSuccess(controller);
                triggerProp.OnEnter(controller.Pawn, mono);
            }
            else
                OnTriggerFail(controller);
        }

        private void TriggerExitAction(PlayerController controller)
        {
            if (CheckAvailable(controller.Pawn))
                OnTriggerSuccess(controller, true);                
            else
                OnTriggerFail(controller, true);
            triggerProp.OnExit(controller.Pawn, mono);
        }

        protected virtual bool CheckAvailable(CharacterPawn pawn)
        {
            if (triggerProp.Checker != null)
                return triggerProp.Checker.CheckPlayer(pawn, this);

            return true;
        }
        public virtual void OnTriggerSuccess(PlayerController controller, bool bExit = false)
        {
            //mono.GetComponent<Collider>().enabled = false;
            bool bTriggered = false;
            var eventList = bExit ? triggerProp.ExitEvents : triggerProp.EntryEvents;
            if (eventList != null)
            {
                if(targets == null)
                {
                    targets = new List<CharacterPawn>();
                }
                foreach (Property.EventSequence sequence in eventList)
                {
                    //run trigger event
                    bTriggered = true;
                    triggerProp.FindTarget(controller, this, ref targets);
                    foreach (CharacterPawn pawn in targets)
                    {
                        sequence.CheckAndExecute(pawn, this);
                    }
                }
            }
            if (bTriggered && triggerProp.UseForOnce && activatePawns != null && controller.Pawn != null)
                activatePawns.Add(controller.Pawn);
        }
        protected virtual void OnTriggerFail(PlayerController controller, bool bExit = false)
        {
            //trigger fail

        }

        public void CallEventSequence(PlayerController controller, Property.TargetFinder finder, Property.EventSequence sequence )
        {
            if (targets == null)
            {
                targets = new List<CharacterPawn>();
            }
            targets.Clear();
            finder.FindTarget(controller, this, ref targets);
            foreach (CharacterPawn pawn in targets)
            {
                sequence.CheckAndExecute(pawn, this);
            }
        }

    }
    
}