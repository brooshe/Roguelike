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

    protected override void _OnLoad()
    {
        base._OnLoad();

		mono = actorTrans.GetComponent<TriggerMono>();
		if (mono == null) {
			Debug.LogError ("TriggerMono is null!");
		} else {
			mono.OnTrigger = OnTrigger;
		}
    }

	private void OnTrigger(Collider other)
	{
		CharacterPawn pawn = other.GetComponent<CharacterPawn> ();
		if (pawn != null) 
		{
			if (CheckAvailable (pawn))
				OnTriggerSuccess (pawn);
			else
				OnTriggerFail (pawn);
		}
	}

	protected virtual bool CheckAvailable(CharacterPawn pawn)
	{
		return true;	
	}
	protected virtual void OnTriggerSuccess(CharacterPawn pawn)
    {
        //run trigger event

        
    }
	protected virtual void OnTriggerFail(CharacterPawn pawn)
	{
		//trigger fail

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
