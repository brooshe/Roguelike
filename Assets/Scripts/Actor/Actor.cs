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

	public void Show(bool bShow)
	{
		if (actorModel && actorModel.activeSelf != bShow)
			actorModel.SetActive (bShow);
	}

    public T Clone<T>() where T : Actor, new()
    {
        T actor = ScriptableObject.CreateInstance<T>();
		this.Copy (actor);

        return actor;
    }

	protected virtual void Copy(Actor actor)
	{
		actor.actorPrefab = this.actorPrefab;
	}

    void OnDestroy()
    {
        if (actorModel != null)
            Destroy(actorModel);
        actorTrans = null;
    }
}
