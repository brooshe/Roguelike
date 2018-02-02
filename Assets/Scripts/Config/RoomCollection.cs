using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using Actor;

public class RoomCollection : ScriptableObject
{
	public List<Room> roomList;

	#if UNITY_EDITOR
	[MenuItem("Assets/Config/RoomCollection", false, 0)]
	public static void CreateRoomCollection()
	{
		string path = "Assets";
		foreach(Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			path = AssetDatabase.GetAssetPath(obj);
			if(File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
			}
			break;
		}
		path += "/RoomCollection";
		RoomCollection asset = ScriptableObject.CreateInstance<RoomCollection>();
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
