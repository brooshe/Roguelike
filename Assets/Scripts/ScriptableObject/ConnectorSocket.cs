using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    [System.Serializable]
    public class ConnectorSocket : TriggerSocket
    {
        public enum TYPE
        {
            TYPE_RELATIVE,
            TYPE_ABSTRACT,
        }
        public TYPE socketType = TYPE.TYPE_RELATIVE;

        [SerializeField]
        private IntVector3 m_LogicPosition;
        [HideInInspector]
        public IntVector3 LogicPosition
        {
            get { return m_LogicPosition; }
        }

        [SerializeField]
        private IntVector3 m_ConnectTo;
        [HideInInspector]
        public IntVector3 ConnectPos
        {
            get { return m_ConnectTo; }
        }

        public override ActorInstance.Trigger GenerateTrigger()
        {
            ActorInstance.Connector conn = null;
            if (TriggerType != null)
            {
                conn = new ActorInstance.Connector(TriggerType as Connector);
                conn.socket = this;
            }
            return conn;
        }
    }
}