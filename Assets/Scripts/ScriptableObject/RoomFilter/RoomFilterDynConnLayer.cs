using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "RoomFilter/DynamicConnAndLayer")]
    public class RoomFilterDynConnLayer : RoomFilterDynConn
    {
        [SerializeField]
        private int layer;
        public override bool Check(Room room)
        {
            if (!base.Check(room))
                return false;

            return (room.GetLayer() & layer) != 0;                
        }
    }

}