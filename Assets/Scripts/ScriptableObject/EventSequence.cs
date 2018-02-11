using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    public enum SequencePassType
    {
        CHECK_PASS,
        CHECK_FAIL,
    }
    [CreateAssetMenu(menuName = "EventSequence/General")]
    public class EventSequence : ScriptableObject
    {
        public Checker check;
        //Timer
        public EventTimer timer;
        public List<EventFunction> pass_functions;
        public List<EventFunction> fail_functions;
        public SequencePassType PassType;

        public void Init()
        {
            if(pass_functions == null)
            {
                foreach (var func in pass_functions)
                {
                    func.Init();
                }                
                return;
            }
            if (fail_functions == null)
            {
                foreach (var func in fail_functions)
                {
                    func.Init();
                }
                return;
            }
        }

        public virtual bool CheckAndExecute(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            bool bPass = check == null || check.CheckPlayer(pawn, actor);
            List<EventFunction> functions = bPass ? pass_functions : fail_functions;
            if (functions != null)
            {
                foreach (var func in functions)
                {
                    if (timer == null)
                        func.Execute(pawn, actor);
                    else
                        GameLoader.Instance.AddTimer(new EventTimerInst(timer, func, pawn, actor));
                }
            }

            return (bPass && PassType == SequencePassType.CHECK_PASS) ||
                    (!bPass && PassType == SequencePassType.CHECK_FAIL);
        }
    }
}