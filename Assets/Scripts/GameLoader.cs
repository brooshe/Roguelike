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

    private Queue<Property.Room> roomQueue;
	private List<Property.Room> repeatRooms;

	public delegate Property.Room PickRoom();
	public PickRoom PickRoomMethod;

    private List<EventTimerInst> Timers = new List<EventTimerInst>();
    public List<Trigger> RemoteTriggers = new List<Trigger>();

    void Awake()
    {
        _instance = this;
    }
    private Room lobby;
	// Use this for initialization
	void Start () {
        FillRoomStack();
		//PickRoomMethod = PickRoomRandomly;

        Property.Room lobbyProp = Resources.Load<Property.Room>("Room/EntranceHall");
        if(lobbyProp)
        {
            lobby = new Room(lobbyProp);
			lobby.Init(IntVector3.Zero, Rotation2D.Identity, Vector3.zero, Quaternion.identity);
			lobby.Show (true);

            Property.Room upperLanding = Resources.Load<Property.Room>("Room/UpperLanding");
            if (upperLanding)
            {
                Room room = new Room(upperLanding);
                room.Init(new IntVector3(0, 1, 3), Rotation2D.Identity, Vector3.zero, Quaternion.identity);
            }
            else
                Debug.LogError("Find UpperLanding fail!!");

            Property.Room basementLanding = Resources.Load<Property.Room>("Room/BasementLanding");
            if (basementLanding)
            {
                Room room = new Room(basementLanding);
                room.Init(new IntVector3(0, -1, 0), Rotation2D.Identity, Vector3.zero, Quaternion.identity);
            }
            else
                Debug.LogError("Find BasementLanding fail!!");

            GameObject charPrefab = Resources.Load<GameObject>("Models/Characters/Ethan");
            GameObject charGO = Instantiate<GameObject>(charPrefab);
            CharacterPawn pawn = charGO.GetComponent<CharacterPawn>();
            Property.CharacterDefine charDef = Resources.Load<Property.CharacterDefine>("Character/TestChar");
            pawn.Setup(charDef);
        }
        else
        {
            Debug.LogError("Find Lobby fail!!");
        }
	}

    public void EnterScene(CharacterPawn pawn)
    {
        Connector connector = lobby.FindEntry(IntVector3.Invalid);
        if (connector != null)
        {
            connector.TryGetThrough(pawn, IntVector3.Invalid);
        }
    }

    private void FillRoomStack()
    {
		roomQueue = new Queue<Property.Room>();
        repeatRooms = new List<Property.Room>();

        RoomCollection collect = Resources.Load<RoomCollection> ("Config/NormalRoomStack");
        List<Property.Room> roomList = collect.roomList;
#if UNITY_EDITOR
        roomList = new List<Property.Room>();
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
                    UIManager.Instance.AddRoom(occupy);
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

    public Room FindRoom(Property.RoomFilter filter)
    {
        foreach(Room room in roomList.Values)
        {
            if (filter.Check(room.RoomProp))
                return room;
        }
        return null;
    }

    protected bool FindPlace(Property.Room roomProp, out IntVector3 position, out IntVector3 entry)
    {
        foreach (Room room in roomList.Values)
        {
            if (room.ConnectorList != null)
            {
                foreach (Connector conn in room.ConnectorList)
                {
                    if (conn.otherConnector == null && !conn.connectorProp.IsDynamic)
                    {
                        Property.ConnectorSocket connSocket = conn.socket as Property.ConnectorSocket;
                        if (connSocket.socketType == Property.ConnectorSocket.TYPE.TYPE_RELATIVE)
                        {                            
                            position = conn.ConnectToPos;
                            if (roomProp.CanPlaceAt(position))
                            {
                                entry = conn.LogicPosition;
                                return true;
                            }
                        }
                    }
                }
            }
        }
        position = entry = IntVector3.Zero;
        return false;
    }

    public Room CreateRoomAt(IntVector3 position, IntVector3 entry)
    {
        return CreateRoomFromStack(null, true, position, entry);
    }

    public Room CreateRoomByFilter(Property.RoomFilter filter)
    {
        return CreateRoomFromStack(filter, false, IntVector3.Zero, IntVector3.Zero);
    }

    private Room CreateRoomFromStack(Property.RoomFilter filter, bool specified, IntVector3 position, IntVector3 entry)
    {
        if (specified)
        {
            Debug.LogFormat("create room at {0},{1},{2}", position.x, position.y, position.z);
            Room result = null;
            if (roomList.TryGetValue(position, out result) && result != null)
                return null;
        }

        if(roomQueue.Count == 0)
        {
            RefillRoomQueue(15);
        }

		Property.Room roomProp = null;
		bool isRoomGood;
		int count = roomQueue.Count;
		do {
			if (--count < 0)
				break;

            roomProp = roomQueue.Dequeue ();
            isRoomGood = filter == null ? true : filter.Check(roomProp);
            isRoomGood = isRoomGood && (specified ? roomProp.CanPlaceAt (position) : FindPlace(roomProp, out position, out entry));
			if(!isRoomGood)
			{
				roomQueue.Enqueue(roomProp);
                roomProp = null;
            }
		} while(!isRoomGood);

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
            if (c.parentRoom == room)
                return;
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

    private void ShuffleRoom(ref List<Property.Room> list)
    {
        // Fisher_Yates shuffle
        int count = list.Count;
        int j;
        Property.Room temp;
        while (--count > 0)
        {
            j = Random.Range(0, count + 1);
            temp = list[count];
            list[count] = list[j];
            list[j] = temp;
        }
    }

    public void AddTimer(EventTimerInst timer)
    {
        if(timer.timeRemain >= 0)
            Timers.Add(timer);
    }

    public void RegisterRemoteTrigger(Trigger trig)
    {
        RemoteTriggers.Add(trig);
    }

    // Update is called once per frame
    void Update ()
    {
        for(int index = 0; index < Timers.Count; ++index)
        {
            EventTimerInst timer = Timers[index];
            if(timer.Update(Time.deltaTime))
            {
                Timers.Remove(timer);
                --index;
            }
        }		
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
