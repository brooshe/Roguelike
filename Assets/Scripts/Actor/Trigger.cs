using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class Trigger : Actor
{
    [HideInInspector]
    public TriggerMono mono;
    [HideInInspector]
    public TriggerSocket socket;

    [SerializeField]
    private string Function;
    [SerializeField]
    private float Float;
    [SerializeField]
    private int Int;
    [SerializeField]
    private string String;
    [SerializeField]
    private object Object;
    [SerializeField]
    private bool UseForOnce;

    private MethodInfo method;

    protected HashSet<CharacterPawn> activatePawns;

    protected override void _OnLoad()
    {
        base._OnLoad();

        mono = actorTrans.GetComponent<TriggerMono>();
        if (mono == null) {
            Debug.LogError("TriggerMono is null!");
        } else {
            mono.OnEnter = OnTriggerEnter;
            mono.OnExit = OnTriggerExit;
        }

        if (!string.IsNullOrEmpty(Function))
        {
            method = typeof(TriggerEvent).GetMethod(Function, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                Debug.LogErrorFormat("Function {0} doesn't exist!", Function);
            }
        }

        if (UseForOnce)
            activatePawns = new HashSet<CharacterPawn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterPawn pawn = other.GetComponent<CharacterPawn>();
        if (pawn != null && pawn.controller != null)
        {
            if (UseForOnce && activatePawns.Contains(pawn))
                return;

            pawn.controller.TriggerActions.Add(TriggerAction);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        CharacterPawn pawn = other.GetComponent<CharacterPawn>();
        if (pawn != null && pawn.controller != null)
        {
            pawn.controller.TriggerActions.Remove(TriggerAction);
        }
    }

    private void TriggerAction(PlayerController controller)
    {
		if (CheckAvailable(controller))
            OnTriggerSuccess(controller);
        else
            OnTriggerFail(controller);
    }

	protected virtual bool CheckAvailable(PlayerController controller)
    {
        return true;
    }
    protected virtual void OnTriggerSuccess(PlayerController controller)
    {
        //mono.GetComponent<Collider>().enabled = false;

        //run trigger event
        ParameterInfo[] param = method.GetParameters();
        if (param.Length == 2)
        {
            method.Invoke(null, new object[] {controller.Pawn, this});
        }
        else if (param.Length == 3)
        {
            ParameterInfo third = param[2];
            if (third.ParameterType == typeof(System.Single))
            {
                method.Invoke(null, new object[] { controller.Pawn, this, this.Float });
            }
            else if (third.ParameterType == typeof(System.Int32))
            {
                method.Invoke(null, new object[] { controller.Pawn, this, this.Int });
            }
            else if (third.ParameterType == typeof(System.String))
            {
                method.Invoke(null, new object[] { controller.Pawn, this, this.String });
            }
            else if (third.ParameterType == typeof(Object))
            {
                method.Invoke(null, new object[] { controller.Pawn, this, this.Object });
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
        if (activatePawns != null && controller.Pawn != null)
            activatePawns.Add(controller.Pawn);
    }
    protected virtual void OnTriggerFail(PlayerController controller)
    {
        //trigger fail

    }

    protected override void Copy(Actor actor)
    {
        base.Copy(actor);

        Trigger trigger = (Trigger)actor;
        trigger.Function = this.Function;
        trigger.Float = this.Float;
        trigger.Int = this.Int;
        trigger.String = this.String;
        trigger.Object = this.Object;
        trigger.UseForOnce = this.UseForOnce;
    }


#if UNITY_EDITOR
    [MenuItem("Assets/Trigger/CreateTrigger", false, 0)]
    public static void CreateTrigger()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }
        path += "/Trigger";
        Trigger asset = ScriptableObject.CreateInstance<Trigger>();
        int duplicateCount = 0;
        string newPath = path;
        while (File.Exists(newPath + ".asset"))
        {
            newPath = string.Format("{0}_{1}", path, ++duplicateCount);
        }
        AssetDatabase.CreateAsset(asset, newPath + ".asset");
        AssetDatabase.SaveAssets();
    }
#endif
}
