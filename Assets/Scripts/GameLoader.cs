using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

    public static GameLoader Instance
    {
        get { return _instance; }
    }

    public float RoundTime = 30; //each round has 30 seconds

    private static GameLoader _instance;
	private Dictionary<IntVector3, Room> roomList = new Dictionary<IntVector3, Room>();

    private Queue<Room> roomQueue;
	private RoomCollection repeatRooms;

	public delegate Room PickRoom();
	public PickRoom PickRoomMethod;

    void Awake()
    {
        _instance = this;
    }
	// Use this for initialization
	void Start () {
        FillRoomStack();
		PickRoomMethod = PickRoomRandomly;

        Room lobby = Resources.Load<Room>("Room/Lobby");
        if(lobby)
        {
			lobby.Init(IntVector3.Zero, Rotation2D.Identity);
			lobby.Show (true);

            GameObject charPrefab = Resources.Load<GameObject>("Charactors/Ethan");
            GameObject charGO = Instantiate<GameObject>(charPrefab);
            CharacterPawn pawn = charGO.GetComponent<CharacterPawn>();
            pawn.CurMovePointLev = 4;
            pawn.ResetMovePoint();
            Connector connector = lobby.FindEntry(IntVector3.Invalid);
            if(connector)
            {
				connector.TryGetThrough(pawn, IntVector3.Invalid);
            }
        }
	}

    private void FillRoomStack()
    {
		roomQueue = new Queue<Room>();
		RoomCollection collect = Resources.Load<RoomCollection> ("Config/NormalRoomStack");
		//shuffle
		int count = collect.roomList.Count;
		while (count-- > 0) {
			int index = Random.Range (0, count);
			roomQueue.Enqueue (collect.roomList [index]);
			collect.roomList.RemoveAt (index);
		}

		repeatRooms = Resources.Load<RoomCollection>("Config/RepeatRooms");
    }

    public void RegisterRoom(Room room)
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

	public Room CreateRoomAt(IntVector3 position, IntVector3 entry)
    {
		Debug.LogFormat ("create room at {0},{1},{2}", position.x, position.y, position.z);
		Room result = null;
		if (roomList.TryGetValue (position, out result) && result != null)
			return null;

		Room room = null;
		bool isRoomFitPos;
		int count = roomQueue.Count;
		do {
			if (--count < 0)
				break;
			
			room = roomQueue.Dequeue ();
			isRoomFitPos = room.CanPlaceAt (position);
			if(!isRoomFitPos)
			{
				roomQueue.Enqueue(room);
			}
		} while(!isRoomFitPos);

		if (room == null)
		{
			//room queue is running out, pick a room from repeatable roomlist
			room = PickRoomMethod();
		}

		if (room != null) {
			room = room.Clone<Room> ();
			//make sure room has a connector to entry
			Rotation2D roomRot = room.FaceTo(entry - position);
			//TODO: after we have position & rotation, check if this room can place in world
			room.Init (position, roomRot);
		}
			
        return room;
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

	private Room PickRoomRandomly()
	{
		int count = repeatRooms.roomList.Count;
		if (count > 0) {
			int index = Random.Range (0, count - 1);
			return repeatRooms.roomList [index];
		}
		return null;
	}

    // Update is called once per frame
    void Update () {
		
	}

    void OnDestroy()
    {
        _instance = null;
    }
}
