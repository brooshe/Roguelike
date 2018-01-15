using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ConnectorSocket
{
    public enum TYPE
    {
        TYPE_RELATIVE,
        TYPE_ABSTRACT,
    }
    public TYPE socketType = TYPE.TYPE_RELATIVE;
    
    [SerializeField]
    private IntVector3 m_ConnectTo;

    [HideInInspector]
    public IntVector3 ConnectPos
    {
        get { return m_ConnectTo; }
    }

    public Vector3 LocalPosition;
    public Vector3 LocalEulerRotation;

    private Connector _curConnector;
    public Connector curConnector
    {
        get { return _curConnector; }
        set
        {
            if (value)
                value.socket = this;
            _curConnector = value;
        }
    }
}
