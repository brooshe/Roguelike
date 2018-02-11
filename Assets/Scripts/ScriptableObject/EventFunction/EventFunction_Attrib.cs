using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Property
{
    [CreateAssetMenu(menuName = "EventFunction/CharacterAttrib")]
    public class EventFunction_Attrib : EventFunction
    {
        public CharacterAtrribute AttribParam;
        public int value;

        protected override void FindMethod()
        {
            method = typeof(EventExecution).GetMethod(Function, new System.Type[] { typeof(CharacterPawn), typeof(ActorInstance.ActorBase), typeof(CharacterAtrribute), typeof(System.Int32)});
        }

        protected override void SetParam(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            base.SetParam(pawn, actor);
#if UNITY_EDITOR
            ParameterInfo[] param = method.GetParameters();
            if (param.Length != 4)
            {
                Debug.LogErrorFormat("Function {0} param num NOT equal to 4!", Function);
            }
            else if (param[2].ParameterType != typeof(CharacterAtrribute))
            {
                Debug.LogErrorFormat("Function {0} param 2 is NOT CharacterAtrribute!", Function);
            }
            else if (param[3].ParameterType != typeof(System.Int32))
            {
                Debug.LogErrorFormat("Function {0} param 3 is NOT int32!", Function);
            }
#endif
            arrObj.Add(AttribParam);
            arrObj.Add(value);
        }
    }
}