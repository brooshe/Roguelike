using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorMono : TriggerMono 
{
    public List<Transform> SpawnLocation = new List<Transform>();

    public Transform FindPlayerStart()
    {
        if (SpawnLocation.Count > 0)
        {
            return SpawnLocation[0];
        }
        else
            return transform;
    }
}
