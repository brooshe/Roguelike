using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Property;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    private bool bBasement;
    private bool bGround;
    private bool bUpstairs;

    SerializedProperty PropActorPrefab;
    SerializedProperty PropRoomName;
    SerializedProperty PropIdentical;
    SerializedProperty PropExtentMin;
    SerializedProperty PropExtentMax;
    SerializedProperty PropLayer;
    SerializedProperty PropGroupRooms;
    SerializedProperty PropConnectorSockets;
    SerializedProperty PropTriggerSockets;
    SerializedProperty PropEntryEvent;
    SerializedProperty PropExitEvent;
    SerializedProperty PropActualSize;

    //void Awake()
    //{
    //    room = (Room)target;
    //    int layer = room.GetLayer;
    //    bUndreGround = (layer & (int)Room.Layer.UNDERGROUND) != 0;
    //    bGround = (layer & (int)Room.Layer.GROUND) != 0;
    //    bUpstairs = (layer & (int)Room.Layer.UPSTAIRS) != 0;
    //}

    void OnEnable()
    {
        PropRoomName = serializedObject.FindProperty("roomName");
        PropIdentical = serializedObject.FindProperty("Identical");
        PropExtentMin = serializedObject.FindProperty("ExtentMin");
        PropExtentMax = serializedObject.FindProperty("ExtentMax");
        PropActorPrefab = serializedObject.FindProperty("actorPrefab");

        PropLayer = serializedObject.FindProperty("layer");
        int layer = PropLayer.intValue;
        bBasement = (layer & (int)Room.Layer.BASEMENT) != 0;
        bGround = (layer & (int)Room.Layer.GROUND) != 0;
        bUpstairs = (layer & (int)Room.Layer.UPSTAIRS) != 0;

        PropGroupRooms = serializedObject.FindProperty("GroupRooms");
        PropConnectorSockets = serializedObject.FindProperty("ConnectorSockets");
        PropTriggerSockets = serializedObject.FindProperty("TriggerSockets");
        PropEntryEvent = serializedObject.FindProperty("EntryEvent");
        PropExitEvent = serializedObject.FindProperty("ExitEvent");

        PropActualSize = serializedObject.FindProperty("ActualSize");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        serializedObject.Update();

        EditorGUILayout.PropertyField(PropRoomName);
        EditorGUILayout.PropertyField(PropActorPrefab);
        EditorGUILayout.PropertyField(PropIdentical);
        EditorGUILayout.PropertyField(PropExtentMin, true);
        EditorGUILayout.PropertyField(PropExtentMax, true);
        EditorGUILayout.PropertyField(PropActualSize);

        EditorGUILayout.Space();
        GUILayout.Label("Placeable Layer");
        //GUILayout.BeginArea(new Rect(30, 110, 200, 300));
        bBasement = EditorGUILayout.ToggleLeft("Basement", bBasement);
        bGround = EditorGUILayout.ToggleLeft("Ground", bGround);
        bUpstairs = EditorGUILayout.ToggleLeft("Upstairs", bUpstairs);
        PropLayer.intValue = (bBasement ? (int)Room.Layer.BASEMENT : 0) | (bGround ? (int)Room.Layer.GROUND : 0) | (bUpstairs ? (int)Room.Layer.UPSTAIRS : 0);
        //GUILayout.EndArea();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(PropEntryEvent);
        EditorGUILayout.PropertyField(PropExitEvent);

        EditorGUILayout.PropertyField(PropGroupRooms, true);
        EditorGUILayout.PropertyField(PropConnectorSockets, true);
        EditorGUILayout.PropertyField(PropTriggerSockets, true);

        serializedObject.ApplyModifiedProperties();
    }

}
