using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{    
    public abstract class RoomFilter : ScriptableObject
    {
        public virtual bool Check(Room room)
        {
            return true;
        }
    }

}