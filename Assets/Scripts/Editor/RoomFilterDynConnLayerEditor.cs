using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Property;

[CustomEditor(typeof(RoomFilterDynConnLayer))]
public class RoomFilterDynConnLayerEditor : Editor
{
    private bool bBasement;
    private bool bGround;
    private bool bUpstairs;

    SerializedProperty PropLayer;


    void OnEnable()
    {
        PropLayer = serializedObject.FindProperty("layer");
        int layer = PropLayer.intValue;
        bBasement = (layer & (int)Room.Layer.BASEMENT) != 0;
        bGround = (layer & (int)Room.Layer.GROUND) != 0;
        bUpstairs = (layer & (int)Room.Layer.UPSTAIRS) != 0;
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        serializedObject.Update();      

        EditorGUILayout.Space();
        GUILayout.Label("Placeable Layer");
        bBasement = EditorGUILayout.ToggleLeft("Basement", bBasement);
        bGround = EditorGUILayout.ToggleLeft("Ground", bGround);
        bUpstairs = EditorGUILayout.ToggleLeft("Upstairs", bUpstairs);
        PropLayer.intValue = (bBasement ? (int)Room.Layer.BASEMENT : 0) | (bGround ? (int)Room.Layer.GROUND : 0) | (bUpstairs ? (int)Room.Layer.UPSTAIRS : 0);
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }

}
