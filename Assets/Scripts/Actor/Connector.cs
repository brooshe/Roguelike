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
        public Connector otherConnector;

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

        public Property.Connector connectorProp
        {
            get { return property as Property.Connector; }
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

                bool result = false;
                //If link another connector, then get through it; 
                if (otherConnector != null)
                {
                    result = otherConnector.TryGetThrough(pawn, fromRoom);
                }
                if (!result)
                {
                    FindRoomErrorCode err = FindOtherConnector();
                    switch (err)
                    {
                        case FindRoomErrorCode.NoError:
                            result = otherConnector.TryGetThrough(pawn, fromRoom);
                            break;
                        case FindRoomErrorCode.RoomInvalid:
                            //get out of "the house"
                            break;
                        case FindRoomErrorCode.RoomNotConnected:
                            UIManager.Instance.Message("This door is locked from the other side.");                            
                            break;
                        case FindRoomErrorCode.CannotCreateRoom:
                            UIManager.Instance.Message("This door is somehow broken.");
                            break;
                        case FindRoomErrorCode.CreateRoomNotConnected:
                            Debug.LogError("After create a new room, there is still no connector!!");
                            break;
                        default:
                            Debug.LogError("this log is impossible!!");
                            break;
                    }
                }
                if(result)
                {
                    Property.Connector thisProp = connectorProp;
                    Property.Connector otherProp = otherConnector.connectorProp;
                    if (!thisProp.IsPortal && !otherProp.IsPortal)
                    {
                        //open door
                        ConnectorMono connMono = mono as ConnectorMono;
                        if (connMono != null)
                        {
                            connMono.Open();
                        }
                        else
                        {
                            Debug.LogErrorFormat("Connector {0} doesn't have connectorMono!!", connectorProp.name);
                        }
                    }
                    else
                    {
                        parentRoom.Show(false);
                    }
                }
                return result;
            }
            else
            {
                //get in
                if (ConnectType == CONNECTOR_TYPE.ONE_WAY_OUT)
                    return false;

                if(otherConnector != null)
                    otherConnector.OnPlayerExit(pawn.controller);

                Property.Connector thisProp = connectorProp;
                Property.Connector otherProp = otherConnector == null ? null : otherConnector.connectorProp;
                if (thisProp.IsPortal || otherProp == null || otherProp.IsPortal)
                {
                    //transport
                    Transform tran = ((ConnectorMono)mono).FindPlayerStart();
                    //parentRoom.OnPawnEnter(pawn);
                    mono.Show(true);
                    parentRoom.Show(true);
                    pawn.Transport(tran.position, tran.rotation);
                    OnPlayerEnter(pawn.controller);
                    Debug.LogFormat("pawn enter {0} pos:{1},{2},{3}", parentRoom.RoomProp.roomName, LogicPosition.x, LogicPosition.y, LogicPosition.z);

                }
                else
                {
                    //set room transform, then hide the door
                    Transform tran = otherConnector.actorTrans;
                    Quaternion targetRot = flipRot * tran.rotation;
                    Quaternion delta = targetRot * Quaternion.Inverse(Quaternion.Euler(socket.LocalEulerRotation));
                    Vector3 vec = delta * -socket.LocalPosition + tran.position;
                    parentRoom.SetTransform(vec, delta);
                    mono.Show(false);
                    parentRoom.Show(true);
                }

                return true;
            }

        }
        static Quaternion flipRot = Quaternion.Euler(0, 180, 0);
        enum FindRoomErrorCode
        {
            NoError,
            RoomInvalid,
            RoomNotConnected,//dest-room has no connection to this one
            CannotCreateRoom,//cannot create room at the desired position
            CreateRoomNotConnected,
        }

        FindRoomErrorCode FindOtherConnector()
        {
            if(connectorProp.finder != null)
            {
                Room room = GameLoader.Instance.FindRoom(connectorProp.finder);
                if(room == null)
                {
                    //create a room
                    room = GameLoader.Instance.CreateRoomByFilter(connectorProp.finder);
                }
                if(room != null)
                {
                    foreach (Connector conn in room.ConnectorList)
                    {
                        if(conn.connectorProp.IsDynamic)
                        {
                            otherConnector = conn;
                            conn.otherConnector = this;
                            return FindRoomErrorCode.NoError;
                        }
                    }
                    return FindRoomErrorCode.RoomNotConnected;              
                }
                else
                    return FindRoomErrorCode.CannotCreateRoom;
            }
            else
            {
                //check connector-socket, whether there is no room/an unavailable room at the other side
                IntVector3 destRoomPos = ConnectToPos;
                Debug.LogFormat("DestPos {0},{1},{2}", destRoomPos.x, destRoomPos.y, destRoomPos.z);

                if (destRoomPos == IntVector3.Invalid)
                {
                    //get out of game
                    return FindRoomErrorCode.RoomInvalid;
                }

                Room outRoom = GameLoader.Instance.GetRoomByLogicPosition(destRoomPos);
                if (outRoom != null)
                {
                    //assuming any room created has connection to other rooms, 
                    //if the room is unconnected to this connector, it doesn't have connector at this orientation                   
                    return FindRoomErrorCode.RoomNotConnected;
                }
                else
                {
                    outRoom = GameLoader.Instance.CreateRoomAt(destRoomPos, LogicPosition);
                    if (outRoom == null)
                    {
                        return FindRoomErrorCode.CannotCreateRoom;
                    }
                    else if (otherConnector == null)
                    {
                        return FindRoomErrorCode.CreateRoomNotConnected;
                    }
                    return FindRoomErrorCode.NoError;
                }
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
                    }
                }
            }
        }

        private void OnPlayerExit(PlayerController controller)
        {
            var eventList = triggerProp.ExitEvents;
            if (eventList != null)
            {
                if (targets == null)
                {
                    targets = new List<CharacterPawn>();
                }
                foreach (Property.EventSequence sequence in eventList)
                {
                    triggerProp.FindTarget(controller, this, ref targets);
                    foreach (CharacterPawn pawn in targets)
                    {
                        sequence.CheckAndExecute(pawn, this);
                    }
                }
            }
        }

        private void OnPlayerEnter(PlayerController controller)
        {
            var eventList = triggerProp.EntryEvents;
            if (eventList != null)
            {
                if (targets == null)
                {
                    targets = new List<CharacterPawn>();
                }
                foreach (Property.EventSequence sequence in eventList)
                {
                    triggerProp.FindTarget(controller, this, ref targets);
                    foreach (CharacterPawn pawn in targets)
                    {
                        sequence.CheckAndExecute(pawn, this);
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