﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorMono;
using TriggerSocket = ActorProperty.TriggerSocket;
using ConnectorSocket = ActorProperty.ConnectorSocket;
using ActorProperty;


namespace ActorInstance
{
    public class Room : ActorBase
    {
        ActorProperty.Room RoomProp
        {
            get { return property as ActorProperty.Room; }
        }

        public IntVector3 LogicPosition
        {
            get; private set;
        }

        //rotation could affect all connectors & sockets
        public Rotation2D LogicRotation;

        private bool bExplored;
        private bool bInit;
        public bool IsInitialized { get { return bInit; } }

        public RoomMono mono;

        public Room(ActorProperty.Room prop) : base(prop)
        {
            mono = actorTrans.GetComponent<RoomMono>();
            mono.OnEnter = OnTriggerEnter;
            mono.OnExit = OnTriggerExit;
        }

        public List<Connector> ConnectorList;
        public List<Trigger> TriggerList;
        public List<Room> LinkRooms;

        public void Init(IntVector3 logicPos, Rotation2D rot, Vector3 worldPos, Quaternion worldRot)
        {
            if(bInit)
            {
                LogicPosition = logicPos;
                LogicRotation = rot;
                SetTransform(worldPos, worldRot);
                return;
            }

            LogicPosition = logicPos;
            LogicRotation = rot;

            SetTransform(worldPos, Quaternion.identity);

            bInit = true;

            GameLoader.Instance.RegisterRoom(this);

            if (ConnectorList != null)
            {
                foreach (Connector connector in ConnectorList)
                {
                    GameLoader.Instance.ConnectToWorld(connector);
                }
            }

            if (RoomProp.GroupRooms != null)
            {
                foreach (var roomLink in RoomProp.GroupRooms)
                {
                    if (LinkRooms == null)
                        LinkRooms = new List<Room>();
                    Room room = roomLink.CreateLinkedRoom(LogicPosition, LogicRotation, worldPos, worldRot);
                    LinkRooms.Add(room);
                }
            }

            PostRoomCreated(); 
        }


        private void PostRoomCreated()
        {
            this.Show(false);
        }

        public override void Show(bool bShow)
        {
            if (actorModel && actorModel.activeSelf != bShow)
            {
                actorModel.SetActive(bShow);
                if (LinkRooms != null)
                {
                    foreach (var room in LinkRooms)
                    {
                        if (room != null)
                            room.Show(bShow);
                    }
                }
            }
        }

        public Connector FindEntry(IntVector3 entry)
        {
            if (ConnectorList != null)
            {
                foreach (Connector conn in ConnectorList)
                {
                    if (conn != null && conn.ConnectToPos == entry)
                    {
                        return conn;
                    }
                }
            }
            return null;
        }

        public void OnPawnEnter(CharacterPawn pawn)
        {
            if (!bExplored)
            {
                //TODO: trigger RoomEvent if there is
                UIManager.Instance.QuestLog(string.Format("You found a new room at {0},{1},{2}", LogicPosition.x, LogicPosition.y, LogicPosition.z));
                bExplored = true;
            }
            else
                UIManager.Instance.QuestLog(string.Format("You entered a room which located at {0},{1},{2}", LogicPosition.x, LogicPosition.y, LogicPosition.z));

            if(pawn.controller != null)
            {
                UIManager.Instance.SetRoomName(RoomProp.roomName);
            }

            ActorProperty.Room prop = RoomProp;
            if (prop.EntryEvent != null)
                prop.EntryEvent.CheckAndExecute(pawn, this);
            //this.Show(true);
        }

        public void GetExtent(out IntVector3 min, out IntVector3 max)
        {
            min = RoomProp.ExtentMin;
            max = RoomProp.ExtentMax;
        }

        public Rotation2D FaceTo(IntVector3 entry/*relative position*/)
        {
            Rotation2D rot = TurnRot(entry, false);
            if (ConnectorList != null)
            {
                foreach (Connector conn in ConnectorList)
                {
                    ConnectorSocket socket = conn.socket as ConnectorSocket;
                    if (socket.socketType == ConnectorSocket.TYPE.TYPE_RELATIVE && conn.Enterable())
                    {
                        rot *= TurnRot(socket.ConnectPos, true);
                        return rot;
                    }
                }
            }
            return Rotation2D.Identity;
        }

        private Rotation2D TurnRot(IntVector3 point, bool bInvert)
        {
            ActorProperty.Room prop = RoomProp;
            if (point.x < prop.ExtentMin.x)//left
            {
                return bInvert ? Rotation2D.East : Rotation2D.West;
            }
            else if (point.x > prop.ExtentMax.x)//right
            {
                return bInvert ? Rotation2D.West : Rotation2D.East;
            }
            else if (point.z < prop.ExtentMin.z)//back
            {
                return Rotation2D.South;//always turn 180
            }
            else if (point.z > prop.ExtentMax.z)//front
            {
                return Rotation2D.North;//always stay still
            }
            return Rotation2D.Identity;
        }

        private void OnTriggerEnter(Collider collider)
        {
            CharacterPawn pawn = collider.GetComponent<CharacterPawn>();
            if (pawn != null)
                OnPawnEnter(pawn);
        }

        private void OnTriggerExit(Collider collider)
        {

        }

        public bool TryExit(CharacterPawn pawn)
        {
            ActorProperty.Room prop = RoomProp;
            if (prop.ExitEvent != null)
                return prop.ExitEvent.CheckAndExecute(pawn, this);
            return true;
        }

    }

}