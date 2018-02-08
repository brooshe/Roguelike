using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ActorProperty
{
    [CreateAssetMenu(menuName = "EventDefine")]
    public class EventDefine : ScriptableObject
    {
        public string Function;
        public float Float;
        public int Int;
        public string String;
        public object Object;
        public bool TriggerAtExit;

        public MethodInfo method { get; set; }
        public void Init()
        {
            if (!string.IsNullOrEmpty(Function))
            {
                method = typeof(EventExecution).GetMethod(Function, BindingFlags.Public | BindingFlags.Static);
            }            
        }
        public void Execute(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            if(method == null)
            {
                Init();
            }
            if(method != null)
            {
                ParameterInfo[] param = method.GetParameters();
                if (param.Length == 2)
                {
                    method.Invoke(null, new object[] { pawn, actor });
                }
                else if (param.Length == 3)
                {
                    ParameterInfo third = param[2];
                    if (third.ParameterType == typeof(System.Single))
                    {
                        method.Invoke(null, new object[] { pawn, actor, Float });
                    }
                    else if (third.ParameterType == typeof(System.Int32))
                    {
                        method.Invoke(null, new object[] { pawn, actor, Int });
                    }
                    else if (third.ParameterType == typeof(System.String))
                    {
                        method.Invoke(null, new object[] { pawn, actor, String });
                    }
                    else if (third.ParameterType == typeof(Object))
                    {
                        method.Invoke(null, new object[] { pawn, actor, Object });
                    }
                    else
                    {
                        Debug.LogErrorFormat("Function {0} param is invalid!", Function);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Function {0} has more than 3 params!", Function);
                }
            }
            else
            {
                Debug.LogErrorFormat("Function {0} doesn't exist!", Function);
            }
        }
    }
}