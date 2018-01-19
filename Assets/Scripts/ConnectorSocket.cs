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
	private List<Connector> possibleConnector;
    
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

	public void GenerateConnector()
	{
		if (possibleConnector != null && possibleConnector.Count > 0) 
		{
			int index = Random.Range (0, possibleConnector.Count - 1);
			curConnector = possibleConnector [index].Clone<Connector> ();
			curConnector.Load ();
		}
	}

	public bool Enterable()
	{
		if (possibleConnector != null) 
		{
			foreach (Connector c in possibleConnector) 
			{
				if (c.ConnectType == Connector.CONNECTOR_TYPE.TWO_WAY)
					return true;
			}
		}
		return false;
	}
}
