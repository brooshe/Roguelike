using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [CreateAssetMenu(menuName = "TriggerFilter/FilterByRoomOrWindow")]
    public class TriggerFilterByRoomOrWindow : TriggerFilterByRoom
    {
        
        public override bool Check(ActorInstance.Trigger trigger)
        {
            if (base.Check(trigger))
                return true;
            ActorInstance.Connector conn = trigger as ActorInstance.Connector;
            if(conn != null && conn.ConnectType == Connector.CONNECTOR_TYPE.NO_WAY) //window
            {
                if(GameLoader.Instance.GetRoomByLogicPosition(conn.ConnectToPos) == null)
                {
                    //windows which has no room at the other side
                    return true;
                }
            }
            return false;
        }
    }

}