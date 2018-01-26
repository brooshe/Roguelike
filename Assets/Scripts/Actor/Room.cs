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
	private IntVector3 ExtentMin = IntVector3.Zero;
	[SerializeField]
	private IntVector3 ExtentMax = IntVector3.Zero;

	public enum Layer
	{
		UNDERGROUND,
		GROUND,
		UPSTAIRS,
	}
	[SerializeField]
	private Layer layer; 		//for simplify, a room can place in only one layer for now.

    [SerializeField]
    private List<ConnectorSocket> ConnectorSockets;

    [SerializeField]
    private List<TriggerSocket> TriggerSockets;

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

	public void Init(IntVector3 posInScene, Rotation2D rot)
    {
        Load();

        LogicPosition = posInScene;
		LogicRotation = rot;

		SetTransform (Vector3.zero, Quaternion.identity);//all room place at origin, only show it when neccesary
        //init connectors

        foreach (ConnectorSocket socket in ConnectorSockets)
        {
			socket.GenerateTrigger ();
			if (socket.curConnector != null) 
			{
				socket.curConnector.parentRoom = this;
				socket.curConnector.SetParent (this.actorTrans, socket.LocalPosition, Quaternion.Euler (socket.LocalEulerRotation));
            
				GameLoader.Instance.ConnectToWorld (socket.curConnector);
			}
        }

        if(TriggerSockets != null)
        {
            foreach (TriggerSocket socket in TriggerSockets)
            {
                socket.GenerateTrigger();
                if (socket.curTrigger != null)
                {
                    socket.curTrigger.SetParent(this.actorTrans, socket.LocalPosition, Quaternion.Euler(socket.LocalEulerRotation));
                }
            }
        }

        PostRoomCreated();
    }


    private void PostRoomCreated()
    {
        GameLoader.Instance.RegisterRoom(this);
		this.Show (false);
    }

    public Connector FindEntry(IntVector3 entry)
    {
        if(ConnectorSockets != null)
        {
            foreach(ConnectorSocket socket in ConnectorSockets)
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
            UIManager.Instance.QuestLog(string.Format("You found a new room at {0},{1},{2}", LogicPosition.x, LogicPosition.y, LogicPosition.z));
            bExplored = true;
        }
        else
            UIManager.Instance.QuestLog(string.Format("You entered a room which located at {0},{1},{2}", LogicPosition.x, LogicPosition.y, LogicPosition.z));

        this.Show (true);
    }

	public void GetExtent(out IntVector3 min, out IntVector3 max)
	{
		min = ExtentMin;
		max = ExtentMax;
	}

	public bool CanPlaceAt(IntVector3 logicPos)
	{
		if (layer == Layer.UNDERGROUND && logicPos.y == -1)
			return true;
		if (layer == Layer.GROUND && logicPos.y == 0)
			return true;
		if (layer == Layer.UPSTAIRS && logicPos.y == 1)
			return true;

		return false;
	}

	public Rotation2D FaceTo(IntVector3 entry/*relative position*/)
	{
		Rotation2D rot = TurnRot (entry, false);
		if(ConnectorSockets != null)
		{
			foreach(ConnectorSocket socket in ConnectorSockets)
			{
				if (socket.socketType == ConnectorSocket.TYPE.TYPE_RELATIVE && socket.Enterable()) 
				{
					rot *= TurnRot (socket.ConnectPos, true);
					return rot;
				}
			}
		}
		return Rotation2D.Identity;
	}

	private Rotation2D TurnRot(IntVector3 point, bool bInvert)
	{
		if(point.x < ExtentMin.x)//left
		{
			return bInvert ? Rotation2D.East : Rotation2D.West;
		}
		else if(point.x > ExtentMax.x)//right
		{
			return bInvert ? Rotation2D.West : Rotation2D.East;
		}
		else if(point.z < ExtentMin.z)//back
		{
			return Rotation2D.South;//always turn 180
		}
		else if(point.z > ExtentMax.z)//front
		{
			return Rotation2D.North;//always stay still
		}
		return Rotation2D.Identity;
	}

	protected override void Copy(Actor actor)
	{
		base.Copy (actor);
		Room room = actor as Room;
		room.ExtentMin = this.ExtentMin;
		room.ExtentMax = this.ExtentMax;
		room.layer = this.layer;
		if (ConnectorSockets != null) 
		{
			room.ConnectorSockets = new List<ConnectorSocket> (this.ConnectorSockets.Count);
			foreach (ConnectorSocket socket in ConnectorSockets) 
			{
				room.ConnectorSockets.Add (socket);
			}
		}

        if (TriggerSockets != null)
        {
            room.TriggerSockets = new List<TriggerSocket>(this.TriggerSockets.Count);
            foreach (TriggerSocket socket in TriggerSockets)
            {
                room.TriggerSockets.Add(socket);
            }
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
