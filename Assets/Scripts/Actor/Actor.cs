using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : ScriptableObject
{
    [SerializeField]
    private GameObject actorPrefab;

    protected GameObject actorModel;
    protected Transform actorTrans;

    public void Load()
    {
        _OnLoad();
    }

    protected virtual void _OnLoad()
    {
        if (actorPrefab)
        {
            if (actorModel == null)
            {
                actorModel = UnityEngine.Object.Instantiate<GameObject>(actorPrefab);
                if (actorModel)
                    actorTrans = actorModel.transform;
            }
        }
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

    public T Copy<T>() where T : Actor, new()
    {
        T actor = ScriptableObject.CreateInstance<T>();
        actor.actorPrefab = this.actorPrefab;
        actor.Load();

        return actor;
    }

    void OnDestroy()
    {
        if (actorModel != null)
            Destroy(actorModel);
        actorTrans = null;
    }
}
