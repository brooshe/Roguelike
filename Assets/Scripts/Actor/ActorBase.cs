using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ActorInstance
{
    public abstract class ActorBase
    {
        protected ActorProperty.Actor property;
        public GameObject actorModel;
        public Transform actorTrans;

        public ActorBase(ActorProperty.Actor prop)
        {
            property = prop;
            property.Setup(this);
        }


        public void SetTransform(Vector3 location, Quaternion rotation)
        {
            if (actorTrans)
            {
                actorTrans.position = location;
                actorTrans.rotation = rotation;
            }
        }

        public void SetParent(Transform parent, Vector3 localPos, Quaternion localRot)
        {
            if (actorTrans)
            {
                actorTrans.SetParent(parent);
                actorTrans.localPosition = localPos;
                actorTrans.localRotation = localRot;
            }
        }

        public virtual void Show(bool bShow)
        {
            if (actorModel && actorModel.activeSelf != bShow)
                actorModel.SetActive(bShow);
        }

        public void Dispose()
        {
            Debug.Log("Actor Dispose.");
            actorTrans = null;
            if (actorModel != null)
            {
                GameObject.Destroy(actorModel);
                actorModel = null;
            }
        }
    }

}
