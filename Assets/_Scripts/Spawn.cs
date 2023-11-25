using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    Entity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawm;
    int enemiesRemainingAlive;
    float nextSpwanTime;

    MapGenerator map;

    float timeBtwCampingChecks = 2f;
    float campThresholdDist = 0.5f;
    float nextCampCheckTime;
    Vector3 campPosOld;
    bool isCamping;

    public event System.Action <int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBtwCampingChecks + Time.deltaTime;
        campPosOld = playerT.position;

        map = FindObjectOfType<MapGenerator>();
        NextWave();

    }

    private void Update()
    {
        if(Time.time > nextCampCheckTime && playerT != null)
        {
            nextCampCheckTime = Time.time + timeBtwCampingChecks;

            isCamping = (Vector3.Distance(playerT.position, campPosOld) < campThresholdDist);
            campPosOld = playerT.position;
        }

        if((enemiesRemainingToSpawm > 0 || currentWave.infinite) && Time.time > nextSpwanTime)
        {
            enemiesRemainingToSpawm--;
            nextSpwanTime = Time.time + currentWave.timeBetweenSpawns;

            if(playerT != null)
            {
                StartCoroutine(SpawnEnemy());
            }           

        /*            Debug.Log(enemiesRemainingAlive--);

                    if (enemiesRemainingAlive == 0)
                    {
                        NextWave();
                    }*/

        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform randomTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            randomTile = map.GetTileFromPos(playerT.position);
        }

        Material tilMat = randomTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTime = 0;

        while (spawnTime < spawnDelay)
        {
            tilMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTime * tileFlashSpeed, 1));

            spawnTime += Time.deltaTime;
            
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }



    void OnEnemyDeath()
    {
        Debug.Log("Dead");

        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPos()
    {
        playerT.position = map.GetTileFromPos(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        if(currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }
        currentWaveNumber++;

        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves [currentWaveNumber - 1];

            enemiesRemainingToSpawm = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawm;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }

            ResetPlayerPos();
        }
     
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;

        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}
