using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "RoomFilter/DynamicConnector")]
    public class RoomFilterDynConn : RoomFilter
    {
        public override bool Check(Room room)
        {
            if (!base.Check(room))
                return false;

            foreach (ConnectorSocket socket in room.ConnectorSockets)
            {
                Connector conn = socket.TriggerType as Connector;
                if (conn != null && conn.IsDynamic)
                    return true;
            }
            return false;
        }
    }

}