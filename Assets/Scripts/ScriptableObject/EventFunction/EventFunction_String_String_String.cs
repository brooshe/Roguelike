using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/String_String_String")]
    public class EventFunction_String_String_String : EventFunction
    {
        public string strParam0;
        public string strParam1;
        public string strParam2;

        protected override void FindMethod()
        {
            method = typeof(EventExecution).GetMethod(Function, new System.Type[] { typeof(CharacterPawn), typeof(ActorInstance.ActorBase), typeof(System.String), typeof(System.String), typeof(System.String) });
        }
        protected override void SetParam(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            base.SetParam(pawn, actor);
#if UNITY_EDITOR
            ParameterInfo[] param = method.GetParameters();
            if (param.Length != 5)
            {
                Debug.LogErrorFormat("Function {0} param num NOT equal to 3!", Function);
            }
            else if (param[2].ParameterType != typeof(System.String))
            {
                Debug.LogErrorFormat("Function {0} param 2 is NOT string!", Function);
            }
            else if (param[3].ParameterType != typeof(System.String))
            {
                Debug.LogErrorFormat("Function {0} param 3 is NOT string!", Function);
            }
            else if (param[4].ParameterType != typeof(System.String))
            {
                Debug.LogErrorFormat("Function {0} param 4 is NOT string!", Function);
            }
#endif
            arrObj.Add(strParam0);
            arrObj.Add(strParam1);
            arrObj.Add(strParam2);
        }
    }
}