using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class Barrier : Trigger
{
    private Collider collider;
    protected override void _OnLoad()
    {
        base._OnLoad();

        mono.OnEnter = this.OnTriggerEnter;
        mono.OnExit = this.OnTriggerExit;

        collider = mono.transform.GetChild(0).GetComponent<Collider>();
    }

    private CharacterPawn consumePawn;
    //private Vector3 collideLocation;

    private void OnTriggerEnter(Collider other)
    {
        CharacterPawn pawn = other.GetComponent<CharacterPawn>();
        if (pawn != null && pawn.controller != null)
        {
            if (pawn.CurMovePoint > 0)
            {
                consumePawn = pawn;
                //collideLocation = pawn.transform.position;
                Physics.IgnoreCollision(other, collider, true);                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        CharacterPawn pawn = other.GetComponent<CharacterPawn>();
        if (pawn != null && pawn.controller != null)
        {
            if (pawn == consumePawn)
            {
                consumePawn.ConsumeMovePoint(1);
                consumePawn = null;
            }
            Physics.IgnoreCollision(other, collider, false);
        }
    }


#if UNITY_EDITOR
    [MenuItem("Assets/Trigger/CreateBarrier", false, 0)]
    public static void CreateBarrier()
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
        path += "/Barrier";
        Barrier asset = ScriptableObject.CreateInstance<Barrier>();
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
