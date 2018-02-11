using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/String")]
    public class EventFunction_String : EventFunction
    {
        public string strParam;

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
#endif
            arrObj.Add(strParam);
        }
    }
}