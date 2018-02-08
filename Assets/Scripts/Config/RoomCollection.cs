using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorProperty;

[CreateAssetMenu(menuName = "Config/RoomCollection")]
public class RoomCollection : ScriptableObject
{
	public List<Room> roomList;
}
