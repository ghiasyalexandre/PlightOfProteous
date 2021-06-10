using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;

    public ObjectPoolItem(GameObject obj, int amt, bool exp = true)
    {
        objectToPool = obj;
        amountToPool = Mathf.Max(amt, 2);
        shouldExpand = exp;
    }
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<ObjectPoolItem> itemsToPool;

    public List<List<GameObject>> pooledObjectsList;
    public List<GameObject> pooledObjects;
    private List<int> positions;

    void Awake()
    {
        if (SharedInstance == null)
            SharedInstance = this;
        else if (SharedInstance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        pooledObjectsList = new List<List<GameObject>>();
        pooledObjects = new List<GameObject>();
        positions = new List<int>();

        for (int i = 0; i < itemsToPool.Count; i++)
        {
            ObjectPoolItemToPooledObject(i);
        }
    }

    private void Start()
    {
        for(int i = 0; i < itemsToPool[0].amountToPool; i++)
        {
            var obj = Instantiate(GetPooledObject(0));
            
            obj.SetActive(true);
            obj.SetActive(false);
        }
    }

    public GameObject GetPooledObject(int index)
    {
        int curSize = pooledObjectsList[index].Count;
        for (int i = positions[index] + 1; i < positions[index] + pooledObjectsList[index].Count; i++)
        {

            if (pooledObjectsList[index][i % curSize].activeSelf == false)
            {
                positions[index] = i % curSize;
                //Debug.Log("Adding Objects: " + pooledObjectsList[index][i % curSize].name);
                return pooledObjectsList[index][i % curSize];
            }
        }

        if (itemsToPool[index].shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(itemsToPool[index].objectToPool);
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            pooledObjectsList[index].Add(obj);
            //Debug.Log("Need More of Object: " + obj.name);
            return obj;
        }
        return null;
    }

    public List<GameObject> GetAllPooledObjects(int index)
    {
        return pooledObjectsList[index];
    }

    public int AddObject(GameObject GO, int amt = 10, bool exp = true)
    {
        ObjectPoolItem item = new ObjectPoolItem(GO, amt, exp);
        int currLen = itemsToPool.Count;
        itemsToPool.Add(item);
        ObjectPoolItemToPooledObject(currLen);
        return currLen;
    }

    void ObjectPoolItemToPooledObject(int index)
    {
        ObjectPoolItem item = itemsToPool[index];

        pooledObjects = new List<GameObject>();
        for (int i = 0; i < item.amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(item.objectToPool);
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            pooledObjects.Add(obj);
        }

        pooledObjectsList.Add(pooledObjects);
        positions.Add(0);
    }
}