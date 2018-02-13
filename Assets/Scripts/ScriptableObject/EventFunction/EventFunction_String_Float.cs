using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/String_Float")]
    public class EventFunction_String_Float : EventFunction
    {
        public string StringParam;
        public float FloatParam;

        protected override void FindMethod()
        {
            method = typeof(EventExecution).GetMethod(Function, new System.Type[] { typeof(CharacterPawn), typeof(ActorInstance.ActorBase), typeof(System.String), typeof(System.Single) });
        }
        protected override void SetParam(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            base.SetParam(pawn, actor);
#if UNITY_EDITOR
            ParameterInfo[] param = method.GetParameters();
            if (param.Length != 3)
            {
                Debug.LogErrorFormat("Function {0} param num NOT equal to 3!", Function);
            }
            else if (param[2].ParameterType != typeof(System.String))
            {
                Debug.LogErrorFormat("Function {0} param 2 is NOT string!", Function);
            }
            else if (param[3].ParameterType != typeof(System.Single))
            {
                Debug.LogErrorFormat("Function {0} param 3 is NOT float!", Function);
            }
#endif
            arrObj.Add(StringParam);
            arrObj.Add(FloatParam);
            
        }
    }
}