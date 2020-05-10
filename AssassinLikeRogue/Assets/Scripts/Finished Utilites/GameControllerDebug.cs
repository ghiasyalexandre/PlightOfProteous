using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class GameControllerDebug : MonoBehaviour
{
    public KeyCode itemKey = KeyCode.V;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveGameKey = KeyCode.LeftBracket;
    public KeyCode loadGameKey = KeyCode.RightBracket;
    public float spawnRadius = 5f;
    public Transform spawnPoint;
    //public Transform item;
    //public Transform prefab;
    public List<Transform> objects;
    public int indexToSpawn;
    ObjectPooler pooler;

    string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");

        pooler = ObjectPooler.SharedInstance;

        //objects = new List<Transform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            SpawnEnemy(indexToSpawn);
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveGameKey))
        {
            Save();
        }
        else if (Input.GetKeyDown(loadGameKey))
        {
            Load();
        }
        else if (Input.GetKeyDown(itemKey))
        {
            SpawnItem();
        }
    }

    public void BeginNewGame()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear(); // Removes List reference
    }

    void Save()
    {
        using (
                var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
        ){
            writer.Write(objects.Count);

            for (int i = 0; i < objects.Count; i++)
            {
                Transform t = objects[i];
                writer.Write(t.localPosition.x);
                writer.Write(t.localPosition.y);
                writer.Write(t.localPosition.z);
                writer.Write(t.gameObject.GetComponent<Health>().GetHealth());
            }
        }
    }

    void Load()
    {
        BeginNewGame();
        using (
                var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
            ) {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                float hp;
                Vector3 p;
                p.x = reader.ReadSingle();
                p.y = reader.ReadSingle();
                p.z = reader.ReadSingle();
                hp = reader.ReadSingle();
                Transform t = pooler.GetPooledObject(2).transform;
                t.localPosition = p;
                t.gameObject.GetComponent<Health>().SetHealth(hp);
                objects.Add(t);
            }
        }
    }

    private void SpawnEnemy(int _index = 2)
    {
        GameObject prefabInstance = pooler.GetPooledObject(_index);
        prefabInstance.SetActive(true);
        var behaviourScript = prefabInstance.GetComponent<EnemyAI>();
        prefabInstance.GetComponent<Collider2D>().enabled = true;
        behaviourScript.enabled = true;
        prefabInstance.GetComponentInChildren<Canvas>().gameObject.SetActive(true);
        prefabInstance.transform.rotation = Quaternion.identity;
        prefabInstance.transform.position = new Vector3(
            Random.Range(-9f, 10f),
            Random.Range(-7f, 7f),
            0f);
    }

    public void SpawnItem()
    {
        GameObject t = pooler.GetPooledObject(3);
        t.transform.rotation = Quaternion.identity;
        t.transform.position = Random.insideUnitCircle * spawnRadius;
    }
}
