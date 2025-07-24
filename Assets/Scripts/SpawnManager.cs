using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float spawnRange = 9.0f;
    public int enemyCount;
    public int waveNumber = 1;
    public GameObject[] enemyPrefabs;
    public GameObject[] powerupPrefabs;
    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefab;
    public int bossRound = 2;
    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        SpawnEnemyWave(waveNumber);
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0 && !playerControllerScript.gameOver)
        {
            waveNumber++;
            // Spawn a boss every x number of wares
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWare(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }
            // Updated to select a random powerup prefab for the Medium Challenge
            int randomPowerup = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomEnemy], GenerateSpawnPosition(), enemyPrefabs[randomEnemy].transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    void SpawnBossWare(int currentRound)
    {
        int miniEnemysToSpawn;
        // We dont want to divide by 0!
        if (bossRound != 0)
        {
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }
        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefab.Length);
            Instantiate(miniEnemyPrefab[randomMini], GenerateSpawnPosition(), miniEnemyPrefab[randomMini].transform.rotation);
        }
    }
}
