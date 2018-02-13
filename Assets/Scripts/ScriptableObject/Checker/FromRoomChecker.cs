using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "Check/FromRoom")]
    public class FromRoomChecker : Checker
    {
        public List<string> RoomFromList;

        public override bool CheckPlayer(CharacterPawn pawn, ActorInstance.ActorBase actor)
        {
            ActorInstance.Connector door = actor as ActorInstance.Connector;
            if(door != null && door.otherConnector != null)
            {
                string fromRoom = door.otherConnector.parentRoom.RoomProp.roomName;
                foreach (string name in RoomFromList)
                {
                    if (fromRoom.Equals(name))
                        return true;
                }
            }
            return false;
        }
    }
}