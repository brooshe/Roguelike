﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorMono
{
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

        public void Open()
        {
            Animator anim = GetComponent<Animator>();
            anim.SetBool("open", true);
        }
    }
}
