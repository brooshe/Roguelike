using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "TriggerFilter/FilterByRoom")]
    public class TriggerFilterByRoom : TriggerFilter
    {
        public List<string> NameList;
        public override bool Check(ActorInstance.Trigger trigger)
        {            
            if(NameList != null)
            {
                foreach(string name in NameList)
                {
                    if (trigger.parentRoom.RoomProp.roomName.Equals(name))
                        return true;
                }
            }
            return false;            
        }
    }

}