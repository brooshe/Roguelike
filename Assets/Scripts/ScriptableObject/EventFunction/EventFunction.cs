using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/Simple")]
    public class EventFunction : ScriptableObject
    {
        public string Function;

        public MethodInfo method { get; set; }
        public void Init()
        {
            if (!string.IsNullOrEmpty(Function))
            {
                FindMethod();                
            }
            if (arrObj == null)
                arrObj = new List<object>();
        }
        protected virtual void FindMethod()
        {
            method = typeof(EventExecution).GetMethod(Function, new System.Type[] {typeof(CharacterPawn), typeof(ActorInstance.ActorBase) });
        }
        protected static List<object> arrObj;
        public void Execute(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            if(method == null)
            {
                Init();
            }
            if(method != null)
            {
                SetParam(pawn, actor);               
                method.Invoke(null, arrObj.ToArray());
            }
            else
            {
                Debug.LogErrorFormat("Function {0} doesn't exist!", Function);
            }
        }

        protected virtual void SetParam(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            arrObj.Clear();
            arrObj.Add(pawn);
            arrObj.Add(actor);
        }
    }
}