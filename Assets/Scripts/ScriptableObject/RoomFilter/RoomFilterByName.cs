using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "RoomFilter/ByName")]
    public class RoomFilterByName : RoomFilter
    {
        public List<string> NameList;
        public override bool Check(Room room)
        {
            if(NameList != null)
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