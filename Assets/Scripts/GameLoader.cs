using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorMono;
using ActorInstance;

public class GameLoader : MonoBehaviour {

    public static GameLoader Instance
    {
        get { return _instance; }
    }

    public float RoundTime = 30; //each round has 30 seconds

    private static GameLoader _instance;
	private Dictionary<IntVector3, ActorInstance.Room> roomList = new Dictionary<IntVector3, ActorInstance.Room>();

    private Queue<ActorProperty.Room> roomQueue;
	private List<ActorProperty.Room> repeatRooms;

	public delegate ActorProperty.Room PickRoom();
	public PickRoom PickRoomMethod;

    void Awake()
    {
        _instance = this;
    }
	// Use this for initialization
	void Start () {
        FillRoomStack();
		//PickRoomMethod = PickRoomRandomly;

        ActorProperty.Room lobbyProp = Resources.Load<ActorProperty.Room>("Room/EntranceHall");
        if(lobbyProp)
        {
            Room lobby = new Room(lobbyProp);
			lobby.Init(IntVector3.Zero, Rotation2D.Identity, Vector3.zero, Quaternion.identity);
			lobby.Show (true);

            GameObject charPrefab = Resources.Load<GameObject>("Models/Characters/Ethan");
            GameObject charGO = Instantiate<GameObject>(charPrefab);
            CharacterPawn pawn = charGO.GetComponent<CharacterPawn>();
            ActorProperty.CharacterDefine charDef = Resources.Load<ActorProperty.CharacterDefine>("Character/TestChar");
            pawn.Setup(charDef);
            //pawn.CurMovePointLev = 4;
            //pawn.ResetMovePoint();
            Connector connector = lobby.FindEntry(IntVector3.Invalid);
            if(connector != null)
            {
				connector.TryGetThrough(pawn, IntVector3.Invalid);
            }

            ActorProperty.Room upperLanding = Resources.Load<ActorProperty.Room>("Room/UpperLanding");
            if (upperLanding)
            {
                Room room = new Room(upperLanding);
                room.Init(new IntVector3(0, 1, 3), Rotation2D.Identity, Vector3.zero, Quaternion.identity);
            }
            else
                Debug.LogError("Find UpperLanding fail!!");
        }
        else
        {
            Debug.LogError("Find Lobby fail!!");
        }
	}

    private void FillRoomStack()
    {
		roomQueue = new Queue<ActorProperty.Room>();
        repeatRooms = new List<ActorProperty.Room>();

        RoomCollection collect = Resources.Load<RoomCollection> ("Config/NormalRoomStack");
        List<ActorProperty.Room> roomList = collect.roomList;
#if UNITY_EDITOR
        roomList = new List<ActorProperty.Room>();
        roomList.AddRange(collect.roomList);
#endif
        ShuffleRoom(ref roomList);
		//shuffle
		foreach(var room in roomList)
        {            
			roomQueue.Enqueue (room);

            if (!room.Identical)
                repeatRooms.Add(room);
		}
    }

    public void RegisterRoom(ActorInstance.Room room)
    {
		IntVector3 min, max;
		room.GetExtent (out min, out max);
		for (int i = min.x; i <= max.x; ++i) {
			for (int j = min.y; j <= max.y; ++j) {
				for (int k = min.z; k <= max.z; ++k) {
					IntVector3 occupy = new IntVector3 (i,j,k);
					occupy = room.LogicRotation * occupy + room.LogicPosition;
					roomList.Add (occupy, room);
				}
			}
		}
    }

    public Room GetRoomByLogicPosition(IntVector3 position)
    {
		Room result = null;
		roomList.TryGetValue (position, out result);

        return result;
    }

	public Room CreateRoomFromStack(IntVector3 position, IntVector3 entry)
    {
		Debug.LogFormat ("create room at {0},{1},{2}", position.x, position.y, position.z);
		Room result = null;
		if (roomList.TryGetValue (position, out result) && result != null)
			return null;

        if(roomQueue.Count == 0)
        {
            RefillRoomQueue(15);
        }

		ActorProperty.Room roomProp = null;
		bool isRoomFitPos;
		int count = roomQueue.Count;
		do {
			if (--count < 0)
				break;

            roomProp = roomQueue.Dequeue ();
			isRoomFitPos = roomProp.CanPlaceAt (position);
			if(!isRoomFitPos)
			{
				roomQueue.Enqueue(roomProp);
			}
		} while(!isRoomFitPos);

        Room roomInst = null;
		if (roomProp != null) {
            roomInst = new Room(roomProp);
			//make sure room has a connector to entry
			Rotation2D roomRot = roomInst.FaceTo(entry - position);
            //TODO: after we have position & rotation, check if this room can be placed in scene
            roomInst.Init (position, roomRot, Vector3.zero, Quaternion.identity);
		}
			
        return roomInst;
    }

	public void ConnectToWorld(Connector c)
	{
		Room room = null;
		if (roomList.TryGetValue (c.ConnectToPos, out room) && room != null) {
			Connector foundConnector = room.FindEntry (c.LogicPosition);
			if (foundConnector != null) 
			{
				c.otherConnector = foundConnector;
				foundConnector.otherConnector = c;
			}
		}
	}

    private void RefillRoomQueue(int num)
    {
        roomQueue.Clear();
        if (num <= 0)
            return;
        ShuffleRoom(ref repeatRooms);

        foreach (var room in repeatRooms)
        {
            roomQueue.Enqueue(room);
            if (--num <= 0)
                return;
        }
    }

    private void ShuffleRoom(ref List<ActorProperty.Room> list)
    {
        // Fisher_Yates shuffle
        int count = list.Count;
        int j;
        ActorProperty.Room temp;
        while (--count > 0)
        {
            j = Random.Range(0, count + 1);
            temp = list[count];
            list[count] = list[j];
            list[j] = temp;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnDestroy()
    {
        Debug.Log("GameLoader destroyed");
        var e = roomList.GetEnumerator();
        while(e.MoveNext())
        {
            Room room = e.Current.Value;
            room.Dispose();       
        }
        roomList.Clear();
    }

    void OnApplicationQuit()
    {
        _instance = null;
    }
}
