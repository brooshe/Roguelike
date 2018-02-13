using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "RoomFilter/DynamicConnAndName")]
    public class RoomFilterDynConnName : RoomFilterDynConn
    {
        public List<string> NameList;
        public override bool Check(Room room)
        {
            if (!base.Check(room))
                return false;

            if (NameList != null)
            {
                foreach (string name in NameList)
                {
                    if (room.roomName.Equals(name))
                        return true;
                }
            }
            return false;
        }
    }

}