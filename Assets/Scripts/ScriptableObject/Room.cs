using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "Room/Room")]
    public class Room : Actor
    {
        public string roomName;

        public bool Identical;
        public IntVector3 ExtentMin = IntVector3.Zero;
        public IntVector3 ExtentMax = IntVector3.Zero;

        public enum Layer
        {
            BASEMENT = 0x001,
            GROUND = 0x002,
            UPSTAIRS = 0x004,
        }
        [SerializeField]
        private int layer;

        public List<RoomLink> GroupRooms;

        [SerializeField]
        private List<ConnectorSocket> ConnectorSockets;

        [SerializeField]
        private List<TriggerSocket> TriggerSockets;

        public EventSequence EntryEvent;
        public EventSequence ExitEvent;

        protected override void _OnLoad(ActorInstance.ActorBase actor)
        {
            base._OnLoad(actor);

            ActorInstance.Room roomInst = actor as ActorInstance.Room;
            int index = 0;
            //init connectors
            foreach (ConnectorSocket socket in ConnectorSockets)
            {
                ActorInstance.Trigger trig = socket.GenerateTrigger();
                ActorInstance.Connector connector = trig as ActorInstance.Connector;
                if (roomInst.ConnectorList == null)
                    roomInst.ConnectorList = new List<ActorInstance.Connector>();

                if (connector == null)
                {
                    Debug.LogErrorFormat("{0} connector {1} is null!", roomName, index);
                }
                else
                {
                    roomInst.ConnectorList.Add(connector);
                    connector.parentRoom = roomInst;
                    connector.SetParent(roomInst.actorTrans, socket.LocalPosition, Quaternion.Euler(socket.LocalEulerRotation));
                }
                index++;
            }

            //init triggers
            foreach (TriggerSocket socket in TriggerSockets)
            {
                ActorInstance.Trigger trigger = socket.GenerateTrigger();

                trigger.SetParent(roomInst.actorTrans, socket.LocalPosition, Quaternion.Euler(socket.LocalEulerRotation));                
            }
            
        }

        public bool CanPlaceAt(IntVector3 logicPos)
        {
            if ((layer & (int)Layer.BASEMENT) != 0 && logicPos.y == -1)
                return true;
            if ((layer & (int)Layer.GROUND) != 0 && logicPos.y == 0)
                return true;
            if ((layer & (int)Layer.UPSTAIRS) != 0 && logicPos.y == 1)
                return true;

            return false;
        }

    }

}