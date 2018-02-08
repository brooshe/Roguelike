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

        public Property.Trigger triggerProp
        {
            get { return property as Property.Trigger; }
        }
        //if use-for-once, those ones who trigger this must be recorded
        protected HashSet<CharacterPawn> activatePawns;

        public Trigger(Property.Trigger prop) : base(prop)
        {
            mono = actorTrans.GetComponent<TriggerMono>();
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
            if (CheckAvailable(controller))
            {
                OnTriggerSuccess(controller);
                triggerProp.OnEnter(controller.Pawn, mono);
            }
            else
                OnTriggerFail(controller);
        }

        private void TriggerExitAction(PlayerController controller)
        {
            if (CheckAvailable(controller))
                OnTriggerSuccess(controller, true);                
            else
                OnTriggerFail(controller, true);
            triggerProp.OnExit(controller.Pawn, mono);
        }

        protected virtual bool CheckAvailable(PlayerController controller)
        {
            if (triggerProp.Checker != null)
                return triggerProp.Checker.CheckPlayer(controller.Pawn, this);

            return true;
        }
        protected virtual void OnTriggerSuccess(PlayerController controller, bool bExit = false)
        {
            //mono.GetComponent<Collider>().enabled = false;
            bool bTriggered = false;
            var eventList = triggerProp.EventList;
            if (eventList != null)
            {
                foreach (var function in eventList)
                {
                    if (function.TriggerAtExit == bExit &&  function.method != null)
                    {
                        bTriggered = true;

                        //run trigger event
                        function.Execute(controller.Pawn, this);                        
                    }
                }
            }
            if (bTriggered && activatePawns != null && controller.Pawn != null)
                activatePawns.Add(controller.Pawn);
        }
        protected virtual void OnTriggerFail(PlayerController controller, bool bExit = false)
        {
            //trigger fail

        }

    }
    
}