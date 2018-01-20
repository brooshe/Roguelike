using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    
    public Connector curConnector
    {
        get { return _curTrigger as Connector; }
        set
        {
            curTrigger = value;
        }
    }

	public bool Enterable()
	{
		if (TriggerType != null) 
		{
            Connector c = TriggerType as Connector;
            if (c != null && c.ConnectType == Connector.CONNECTOR_TYPE.TWO_WAY)
                return true;
        }
		return false;
	}
}
