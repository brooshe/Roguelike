using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class Room : Actor
{
    public bool Identical;
    [SerializeField]
    private List<ConnectorSocket> socketList;
    [SerializeField]
    private List<Connector> connectorPool;

    private List<Connector> connectorList;

    public IntVector3 LogicPosition
    {
        get; private set;
    }

    //rotation could affect all connectors & sockets
    [HideInInspector]
    public Rotation2D LogicRotation;

    private bool bExplored = false;

    protected override void _OnLoad()
    {
        base._OnLoad();        

    }

    public void Init(IntVector3 posInScene)
    {
        Load();
        LogicPosition = posInScene;
        //init connectors
        if(connectorPool != null && connectorPool.Count > 0)
        {
            connectorList = new List<Connector>();
            foreach (ConnectorSocket socket in socketList)
            {
                Connector c = connectorPool[0].Copy<Connector>();
                socket.curConnector = c;
                c.SetParent(this.actorTrans, socket.LocalPosition, Quaternion.Euler(socket.LocalEulerRotation));
                connectorList.Add(c);
                //TODO: connect to current world
            }
        }
        else
        {
            Debug.LogErrorFormat("Room {0} has no connector!", actorModel.name);
        }

        PostRoomCreated();
    }

    private void PostRoomCreated()
    {
        GameLoader.Instance.RegisterRoom(this);
        
    }

    public Connector FindEntry(IntVector3 entry)
    {
        if(socketList != null)
        {
            foreach(ConnectorSocket socket in socketList)
            {
                if(socket.curConnector != null && socket.curConnector.ConnectToPos == entry)
                {
                    return socket.curConnector;
                }
            }
        }
        return null;
    }

    public void OnPawnEnter(CharacterPawn pawn)
    {
        if(!bExplored)
        {
            //TODO: trigger RoomEvent if there is
        }


    }

#if UNITY_EDITOR
    [MenuItem("Assets/Room/CreateRoom", false, 0)]
    public static void CreateRoom()
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
        path += "/Room";
        Room asset = ScriptableObject.CreateInstance<Room>();
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
