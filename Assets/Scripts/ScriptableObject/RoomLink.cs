using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Property
{
    [Serializable]
    public class RoomLink
    {
        public string LinkRoomPath;
        public Vector3 Position;
        public Vector3 EulerRotation;
        public IntVector3 LogicPosition;

        public ActorInstance.Room CreateLinkedRoom(IntVector3 logicPos, Rotation2D rot, Vector3 worldPos, Quaternion worldRot)
        {
            //Assuming there's no room-conflict
            ActorInstance.Room linkRoom = GameLoader.Instance.GetRoomByLogicPosition(logicPos + LogicPosition);
            if (linkRoom == null)
            {
                Room roomProp = Resources.Load<Room>(LinkRoomPath);
                if (roomProp != null)
                {
                    ActorInstance.Room instRoom = new ActorInstance.Room(roomProp);
                    instRoom.Init(logicPos + rot * LogicPosition, rot, worldPos + Position, worldRot * Quaternion.Euler(EulerRotation));
                    linkRoom = instRoom;
                }
            }
            
            return linkRoom;
        }
    }
}