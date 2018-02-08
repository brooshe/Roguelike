using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ActorProperty;

[CustomEditor(typeof(Connector))]
public class ConnectorEditor : Editor
{
    SerializedProperty PropActorPrefab;
    SerializedProperty PropConnectType;

    void OnEnable()
    {
        PropActorPrefab = serializedObject.FindProperty("actorPrefab");
        PropConnectType = serializedObject.FindProperty("ConnectType");

    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(PropActorPrefab);
        EditorGUILayout.PropertyField(PropConnectType);

        serializedObject.ApplyModifiedProperties();
    }

}
