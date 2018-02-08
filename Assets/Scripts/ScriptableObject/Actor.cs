using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Property
{
    public abstract class Actor : ScriptableObject
    {
        [SerializeField]
        private GameObject actorPrefab;

        public void Setup(ActorInstance.ActorBase actor)
        {
            _OnLoad(actor);
        }

        protected virtual void _OnLoad(ActorInstance.ActorBase actor)
        {
            if (actorPrefab)
            {
                if (actor.actorModel == null)
                {
                    actor.actorModel = UnityEngine.Object.Instantiate<GameObject>(actorPrefab);
                    if (actor.actorModel != null)
                        actor.actorTrans = actor.actorModel.transform;
                }
            }
        }

    }

}
