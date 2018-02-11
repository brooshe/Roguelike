using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/Float")]
    public class EventFunction_Float : EventFunction
    {
        public float FloatParam;

        protected override void SetParam(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            base.SetParam(pawn, actor);
#if UNITY_EDITOR
            ParameterInfo[] param = method.GetParameters();
            if (param.Length != 3)
            {
                Debug.LogErrorFormat("Function {0} param num NOT equal to 3!", Function);
            }
            else if (param[2].ParameterType != typeof(System.Single))
            {
                Debug.LogErrorFormat("Function {0} param 2 is NOT float!", Function);
            }
#endif
            arrObj.Add(FloatParam);
            
        }
    }
}