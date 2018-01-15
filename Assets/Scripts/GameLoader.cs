using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

    public static GameLoader Instance
    {
        get { return _instance; }
    }
    private static GameLoader _instance;

    private List<Room> roomList = new List<Room>();

    private List<Room> roomStack;
    void Awake()
    {
        _instance = this;
    }
	// Use this for initialization
	void Start () {
        FillRoomStack();

        Room lobby = Resources.Load<Room>("Room/Lobby");
        if(lobby)
        {
            lobby.Init(new IntVector3(0,0,0));
            lobby.SetTransform(Vector3.zero, Quaternion.identity);

            GameObject charPrefab = Resources.Load<GameObject>("Charactors/Ethan");
            GameObject charGO = Instantiate<GameObject>(charPrefab);
            CharacterPawn pawn = charGO.GetComponent<CharacterPawn>();
            Connector connector = lobby.FindEntry(new IntVector3(0, 0, -1));
            if(connector)
            {
                connector.TryGetThrough(pawn, lobby.LogicPosition);
            }
        }
	}

    private void FillRoomStack()
    {
        roomStack = new List<Room>();
        //TODO: fill stack
    }

    public void RegisterRoom(Room room)
    {
        roomList.Add(room);
    }

    public Room GetRoomByLogicPosition(IntVector3 position)
    {
        return null;
    }

    public Room CreateRoomAt(IntVector3 position)
    {
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
