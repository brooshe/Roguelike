using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorMono;
using CONNECTOR_TYPE = Property.Connector.CONNECTOR_TYPE;
using ConnectorSocket = Property.ConnectorSocket;

namespace ActorInstance
{    
    public class Connector : Trigger
    {

        public CONNECTOR_TYPE ConnectType;

        [HideInInspector]
        public Connector otherConnector;
        [HideInInspector]
        public IntVector3 ConnectToPos
        {
            get
            {
                ConnectorSocket connSocket = this.socket as ConnectorSocket;
                if (connSocket == null)
                    Debug.LogError("ConnectToPos: connector's socket is not a ConnectorSocket!");

                if (connSocket.socketType == ConnectorSocket.TYPE.TYPE_ABSTRACT)
                    return connSocket.ConnectPos;
                else
                    return parentRoom.LogicRotation * connSocket.ConnectPos + parentRoom.LogicPosition;
            }
        }
        [HideInInspector]
        public IntVector3 LogicPosition
        {
            get
            {
                ConnectorSocket connSocket = this.socket as ConnectorSocket;
                if (connSocket == null)
                    Debug.LogError("LogicPosition: connector's socket is not a ConnectorSocket!");
                return parentRoom.LogicRotation * connSocket.LogicPosition + parentRoom.LogicPosition;
            }
        }
        
        public Connector(Property.Connector prop) : base(prop)
        {
            ConnectType = prop.ConnectType;
        }

        public bool Available
        {
            get { return ConnectType != CONNECTOR_TYPE.NO_WAY; }
        }

        public bool TryGetThrough(CharacterPawn pawn, IntVector3 fromRoom)
        {
            if (!Available)
                return false;

            Room room = GameLoader.Instance.GetRoomByLogicPosition(fromRoom);
            if (room == parentRoom)
            {
                //get out
                if (ConnectType == CONNECTOR_TYPE.ONE_WAY_IN)
                    return false;

                //If link another connector, then get through it; 
                if (otherConnector != null)
                {
                    return otherConnector.TryGetThrough(pawn, fromRoom);
                }
                //check connector-socket, whether there is no room/an unavailable room at the other side
                IntVector3 destRoomPos = ConnectToPos;
                Debug.LogFormat("DestPos {0},{1},{2}", destRoomPos.x, destRoomPos.y, destRoomPos.z);

                if (destRoomPos == IntVector3.Invalid)
                {
                    //get out of game
                    return false;
                }

                Room destRoom = GameLoader.Instance.GetRoomByLogicPosition(destRoomPos);
                if (destRoom != null)
                {
                    //assuming any room created has connection to other rooms, 
                    //if the room is unconnected to this connector, it doesn't have connector at this orientation                    
                    UIManager.Instance.Message("This door is locked from the other side.");
                    return false;
                }
                else
                {
                    Room createdRoom = GameLoader.Instance.CreateRoomFromStack(destRoomPos, LogicPosition);
                    if(createdRoom == null)
                    {
                        //Debug.LogError("This door cannot be opened.");
                        UIManager.Instance.Message("This door is somehow broken.");
                        return false;
                    }
                    else if (otherConnector == null)
                    {
                        Debug.LogError("After create a new room, there is still no connector!!");
                        return false;
                    }
                    return otherConnector.TryGetThrough(pawn, fromRoom);
                }
            }
            else
            {
                //get in
                if (ConnectType == CONNECTOR_TYPE.ONE_WAY_OUT)
                    return false;

                Transform tran = ((ConnectorMono)mono).FindPlayerStart();
                //parentRoom.OnPawnEnter(pawn);
                parentRoom.Show(true);
                pawn.Transport(tran.position, tran.rotation);
                Debug.LogFormat("pawn enter {0},{1},{2}", LogicPosition.x, LogicPosition.y, LogicPosition.z);
                return true;
            }

        }

        protected override bool CheckAvailable(CharacterPawn pawn)
        {
            if (!Available)
            {
                UIManager.Instance.Message("This door is broken");
                return false;
            }
            if (pawn.RemainMovePoint <= 0)
            {
                UIManager.Instance.Message("Not enough Move-Point!");
                return false;
            }
            return true;
        }

        public override void OnTriggerSuccess(PlayerController controller, bool bExit)
        {
            Debug.Log("Pawn trigger Connector!");
            if(ConnectType == CONNECTOR_TYPE.ONE_WAY_OUT || ConnectType == CONNECTOR_TYPE.TWO_WAY)
            {
                if (parentRoom == null || parentRoom.TryExit(controller.Pawn))
                {
                    if (TryGetThrough(controller.Pawn, LogicPosition))
                    {
                        controller.Pawn.ConsumeMovePoint(1);
                        parentRoom.Show(false);
                    }
                }
            }
        }

        public bool Enterable()
        {
            return ConnectType == CONNECTOR_TYPE.TWO_WAY;
        }

    }
}