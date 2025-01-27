using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public List<GameObject> PrefabsForPool;


    private List<GameObject> _pooledObjects = new List<GameObject>();

    public GameObject GetObjectFromPool(string objectName)
    {
        var instance = _pooledObjects.FirstOrDefault(obj => obj.name == objectName);

        if (instance != null)
        {
            _pooledObjects.Remove(instance);

            instance.SetActive(true);

            return instance;
        }

        var prefab = PrefabsForPool.FirstOrDefault(obj => obj.name == objectName);
        if (prefab != null)
        {

            var newInstance = Instantiate(prefab, Vector2.zero, Quaternion.identity, transform);

            newInstance.name = objectName;

            newInstance.transform.localPosition = Vector2.zero;

            return newInstance;
        }

        Debug.LogWarning("Object pool doesn't have a prefab with the object name " + objectName);
        return null;
    }

    public void PoolObject(GameObject obj)
    {

        obj.SetActive(false);

        _pooledObjects.Add(obj);

    }
}
