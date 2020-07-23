using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { Spawning, Waiting, Counting };

    [SerializeField] private float waveCountDown;
    public float timeBetweenWaves = 15f;
    public float searchCountDown = 1f;

    public float minX, maxX, minY, maxY;

    [System.Serializable]
    public class Wave
    {
        public int index;
        public int count;
        public float rate;
    }

    public TextMeshProUGUI waveCount;
    public TextMeshProUGUI nextWaveCount;
    List<string> namesOfActive = new List<string>();
    List<GameObject> pooledEnemies = new List<GameObject>();
    ObjectPooler pooler;
    public Wave[] waves;
    private int nextWave = 0;
    private int numActiveObj = 0;
    public int NumActiveObj { get { return numActiveObj; } set { numActiveObj = value; } }

    private SpawnState state = SpawnState.Counting;

    private void Start()
    {
        pooler = ObjectPooler.SharedInstance;
        waveCountDown = timeBetweenWaves;    
    }

    private void Update()
    {
        if (state == SpawnState.Waiting)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                // Enemy is Still Alive
                return;
            }
        }

        if (waveCountDown <= 0f)
        {
            if (state != SpawnState.Spawning)
            {
                waveCount.SetText("Wave: " + (nextWave + 1));
                StartCoroutine( SpawnWave ( waves[nextWave] ) );
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
            nextWaveCount.SetText("Next Wave: " + (int)waveCountDown);
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.Spawning;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.index);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.Waiting;

        yield break;
    }

    void WaveCompleted()
    {
        Debug.Log("WAVE COMPLETE!");

        state = SpawnState.Counting;
        waveCountDown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETE!!! Looping...");
        }

        nextWave++;
    }

    private bool EnemyIsAlive()
    {
        bool allInActive = true;
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0f)
        {
            searchCountDown = 1f;
            pooledEnemies = pooler.GetAllPooledObjects(waves[nextWave].index);
            for (int i = 0; i < pooledEnemies.Count; i++)
            {
                if (pooledEnemies[i].activeSelf == true)
                {
                    //numActiveObj++;
                    allInActive = false;
                    break;
                }
            }
        }
        return allInActive;
    }

    private int CountActive()
    {
        if (pooledEnemies == null)
            pooledEnemies = pooler.GetAllPooledObjects(waves[nextWave].index);

        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            if (pooledEnemies[i].activeSelf == true)
            {
                numActiveObj++;
                namesOfActive.Add(pooledEnemies[i].name);
            }
        }
        return numActiveObj;
    }

    private void SpawnEnemy(int _index)
    {
        numActiveObj++;
        GameObject prefabInstance = pooler.GetPooledObject(_index);
        prefabInstance.SetActive(true);
        var behaviourScript = prefabInstance.GetComponent<EnemyAI>();
        behaviourScript.enabled = true;
        prefabInstance.GetComponentInChildren<Canvas>().gameObject.SetActive(true);
        prefabInstance.transform.rotation = Quaternion.identity;
        prefabInstance.transform.position = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f);
    }
}
