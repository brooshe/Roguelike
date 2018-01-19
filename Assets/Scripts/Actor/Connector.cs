﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class Connector : Trigger
{
    public enum CONNECTOR_TYPE
    {
        TWO_WAY,
        ONE_WAY_OUT,
        ONE_WAY_IN,
        NO_WAY,
    }

    public CONNECTOR_TYPE ConnectType = CONNECTOR_TYPE.TWO_WAY;

    [HideInInspector]
    public Room parentRoom;
    [HideInInspector]
    public Connector otherConnector;
    [HideInInspector]
    public ConnectorSocket socket;
    [HideInInspector]
    public IntVector3 ConnectToPos
    {
        get 
		{
            if (socket.socketType == ConnectorSocket.TYPE.TYPE_ABSTRACT)
                return socket.ConnectPos;
            else
                return parentRoom.LogicRotation * socket.ConnectPos + parentRoom.LogicPosition;
        }
    }
	[HideInInspector]
	public IntVector3 LogicPosition
	{
		get 
		{
			return parentRoom.LogicRotation * socket.LogicPosition + parentRoom.LogicPosition;
		}
	}
    protected override void _OnLoad()
    {
        base._OnLoad();


    }

    public bool Available
    {
        get { return ConnectType != CONNECTOR_TYPE.NO_WAY; }
    }

    public bool TryGetThrough(CharacterPawn pawn, IntVector3 fromRoom)
    {
        if (!Available)
            return false;

		Room room = GameLoader.Instance.GetRoomByLogicPosition (fromRoom);
		if(room == parentRoom)
        {
            //get out
            if (ConnectType == CONNECTOR_TYPE.ONE_WAY_IN)
                return false;

            //If link another connector, then get through it; 
            if(otherConnector != null)
            {
                return otherConnector.TryGetThrough(pawn, fromRoom);
            }
            //check connector-socket, whether there is no room/an unavailable room at the other side
            IntVector3 destRoomPos = ConnectToPos;   
			Debug.LogFormat ("DestPos {0},{1},{2}", destRoomPos.x, destRoomPos.y, destRoomPos.z);

			if (destRoomPos == IntVector3.Invalid) {
				//get out of game
				return false;
			}

            Room destRoom = GameLoader.Instance.GetRoomByLogicPosition(destRoomPos);
            if(destRoom != null)
            {
                //assuming any room created has connection to rooms, 
				//if the room is unconnected to this connector, it doesn't have connector at the position
                return false;
            }
            else
            {
				GameLoader.Instance.CreateRoomAt (destRoomPos, LogicPosition);
				if (otherConnector == null) 
				{
					Debug.LogError ("After create a new room, there is still no connector!!");
					return false;
				}
				return otherConnector.TryGetThrough(pawn, fromRoom);
            }
        }
        else
        {
            //get in
            if (ConnectType == CONNECTOR_TYPE.ONE_WAY_OUT)
                return false;

			Transform tran = ((ConnectorMono)mono).FindPlayerStart();
			parentRoom.OnPawnEnter(pawn);
            pawn.Transport(tran.position, tran.rotation);
			Debug.LogFormat ("pawn enter {0},{1},{2}", LogicPosition.x, LogicPosition.y, LogicPosition.z);
            return true;
        }

    }

	protected override bool CheckAvailable(CharacterPawn pawn)
	{
		return this.Available;
	}

	protected override void OnTriggerSuccess(CharacterPawn pawn)
	{
		Debug.Log ("Pawn trigger Connector!");
		if (TryGetThrough (pawn, LogicPosition)) 
		{
			parentRoom.Show (false);
		}

	}

	protected override void Copy(Actor actor)
	{
		base.Copy (actor);

		((Connector)actor).ConnectType = this.ConnectType;
	}

#if UNITY_EDITOR
    [MenuItem("Assets/Connector/CreateConnector", false, 0)]
    public static void CreateConnector()
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
        path += "/Connector";
        Connector asset = ScriptableObject.CreateInstance<Connector>();
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