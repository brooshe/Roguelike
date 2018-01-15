using System.Collections;
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
    [SerializeField]
    protected CONNECTOR_TYPE ConnectType = CONNECTOR_TYPE.TWO_WAY;

    [HideInInspector]
    public ConnectorMono mono;
    [HideInInspector]
    public Room parentRoom;
    [HideInInspector]
    public Connector otherConnector;
    [HideInInspector]
    public ConnectorSocket socket;
    [HideInInspector]
    public IntVector3 ConnectToPos
    {
        get {
            if (socket.socketType == ConnectorSocket.TYPE.TYPE_ABSTRACT)
                return socket.ConnectPos;
            else
                return parentRoom.LogicRotation * socket.ConnectPos + parentRoom.LogicPosition;
        }
    }
    protected override void _OnLoad()
    {
        base._OnLoad();

        mono = actorTrans.GetComponent<ConnectorMono>();
        if(mono == null)
        {
            Debug.LogError("ConnectorMono is null!");
        }
    }

    public bool Available
    {
        get { return ConnectType != CONNECTOR_TYPE.NO_WAY; }
    }

    public bool TryGetThrough(CharacterPawn pawn, IntVector3 fromRoom)
    {
        if (Available)
            return false;

        if(fromRoom == parentRoom.LogicPosition)
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

            Room destRoom = GameLoader.Instance.GetRoomByLogicPosition(destRoomPos);
            if(destRoom != null)
            {
                //assuming any room been created has connection to rooms, if the room is unconnected to this connector, means it doesn't have connector on this side
                return false;
            }
            else
            {
                //create new room
                return true;
            }
        }
        else
        {
            //get in
            if (ConnectType == CONNECTOR_TYPE.ONE_WAY_OUT)
                return false;

            Transform tran = mono.FindPlayerStart();
            pawn.Transport(tran.position, tran.rotation);
            parentRoom.OnPawnEnter(pawn);
            return true;
        }

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
