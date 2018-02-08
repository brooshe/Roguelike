using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorProperty
{
    [CreateAssetMenu(menuName = "Trigger/Connector")]
    public class Connector : Trigger
    {
        public enum CONNECTOR_TYPE
        {
            TWO_WAY,
            ONE_WAY_OUT,
            ONE_WAY_IN,
            NO_WAY,
        }

        public CONNECTOR_TYPE ConnectType = CONNECTOR_TYPE.TWO_WAY;


    }
}