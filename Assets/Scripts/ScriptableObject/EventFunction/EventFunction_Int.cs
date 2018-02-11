using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/Int")]
    public class EventFunction_Int : EventFunction
    {
        public int IntParam;

        protected override void SetParam(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            base.SetParam(pawn, actor);
#if UNITY_EDITOR
            ParameterInfo[] param = method.GetParameters();
            if (param.Length != 3)
            {
                Debug.LogErrorFormat("Function {0} param num NOT equal to 3!", Function);
            }
            else if (param[2].ParameterType != typeof(System.Int32))
            {
                Debug.LogErrorFormat("Function {0} param 2 is NOT int32!", Function);
            }
#endif
            arrObj.Add(IntParam);
        }
    }
}