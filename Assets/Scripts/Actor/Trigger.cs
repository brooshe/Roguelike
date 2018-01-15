using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class Trigger : Actor
{
    protected override void _OnLoad()
    {
        base._OnLoad();
    }

    public bool Activate(CharacterPawn pawn)
    {
        //try to activate this trigger
        return true;
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
