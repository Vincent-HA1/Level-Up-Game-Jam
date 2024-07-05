using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private List<GameObject> Enemies;
    [SerializeField] private float SpawnRadiusSize = 20f;
    [SerializeField] private float TimeBetweenEnemySpawns = 5f;
    [SerializeField] private float TimeBetweenEnemyCampSpawns = 30f;
    [SerializeField] private float TimeBetweenEnemyChanges = 120f;

    [Header("References")]
    [SerializeField] private Transform Player;
    [SerializeField] private WorldGenerator WorldGenerator;
    private Camera MainCamera;

    private float SpawnTimer = 0;
    private float SpawnCampTimer = 0;

    private float GameTimeElapsed = 0;
    private int GameDifficulty = 0;

    private float RadiusAroundSettlement = 5;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        GameTimeElapsed += Time.deltaTime;
        if(GameTimeElapsed > (GameDifficulty + 1) * TimeBetweenEnemyChanges) //every 2 minutes for now i guess
        {
            GameDifficulty += 1;
            TimeBetweenEnemySpawns = Mathf.Clamp(TimeBetweenEnemySpawns - 0.1f, 0, TimeBetweenEnemySpawns);
            TimeBetweenEnemyCampSpawns = Mathf.Clamp(TimeBetweenEnemyCampSpawns - 0.3f, 0, TimeBetweenEnemyCampSpawns);
        }

        SpawnTimer += Time.deltaTime;
        SpawnCampTimer += Time.deltaTime;
        if (SpawnTimer > TimeBetweenEnemySpawns)
        {
            //SpawnEnemyCamp();
            SpawnEnemies();
            SpawnTimer = 0;
        }
        if(SpawnCampTimer > TimeBetweenEnemyCampSpawns)
        {
            SpawnCampTimer = 0;
            SpawnEnemyCamp();
        }
    }

    void SpawnEnemies()
    {
        Vector3 SpawnPoint = GetSpawnPoint();
        while (PositionInCameraBounds(SpawnPoint))
        {
            SpawnPoint = GetSpawnPoint();
        }
        int SpawnIndex = Mathf.Clamp(GameDifficulty, 0, Enemies.Count - 1);
        Instantiate(Enemies[SpawnIndex], SpawnPoint, Quaternion.identity, transform);

    }

    void SpawnEnemyCamp()
    {
        //find a building that is outside of the range.
        //randomly select a settlement (any), then chekc if outside position in camera bounds
        //if so, spawn enemy camp
        //just use random point for now
        Settlement SettlementToGuard = GetSettlementOutOfCameraBounds();//GameObject.Find("Settlement").GetComponent<Settlement>();//GetSettlementOutOfCameraBounds();
        Vector3 SpawnPosition;
        if (SettlementToGuard)
        {
            SpawnPosition = SettlementToGuard.transform.position;
        }
        else
        {
            //get random spawn position if there is no settlement
            SpawnPosition = GetSpawnPoint();
        }
        print("spawn enemy camp");
        //Spawn a random number of enemies proportional to the  GameDifficulty?
        int NumberOfEnemiesToSpawn = Random.Range(2 + GameDifficulty, 5 * GameDifficulty);
        int NumberOfElitesToSpawn = Random.Range(1 + GameDifficulty, 2 + GameDifficulty);
        //Elites are just the next level up of enemies. If there areno eneies left, just instantiate it (but there should always be one level left)
        int NormalEnemyIndex = Mathf.Clamp(GameDifficulty, 0, Enemies.Count - 1);
        int EliteIndex = Mathf.Clamp(GameDifficulty + 1, 0, Enemies.Count - 1);
        int NumberOfEnemiesSpawned = 0;
        int NumberOfElitesSpawned = 0;

        //float NumberOfPositions = 2 * Mathf.PI / (NumberOfEnemiesToSpawn + NumberOfElitesToSpawn) ;

        for (int i = 0; i < NumberOfEnemiesToSpawn + NumberOfElitesToSpawn; i++)
        {
            float RandomAngle = Random.Range(0, 2 * Mathf.PI);
            //Spawn them in a line
            float EliteSpawnChance = Random.Range(0, 3);
            //Check the case where we actually spawn elites
            bool SpawnElites = (EliteSpawnChance == 0 || NumberOfEnemiesSpawned > NumberOfEnemiesToSpawn) && NumberOfElitesSpawned < NumberOfElitesToSpawn;
            //bool SpawnNormalEnemies = (EliteSpawnChance > 0 || NumberOfElitesSpawned > NumberOfElitesToSpawn) && NumberOfEnemiesSpawned < NumberOfEnemiesToSpawn;
            //spawn in a circle, divide the distance by the amount of enemies.
            Vector3 PositionOnCircumference = new Vector3(Mathf.Sin(RandomAngle), Mathf.Cos(RandomAngle)) * RadiusAroundSettlement;
            GameObject EnemySpawned;
            if (SpawnElites)
            {
                EnemySpawned = Instantiate(Enemies[EliteIndex], SpawnPosition + PositionOnCircumference, Quaternion.identity, transform);
                NumberOfElitesSpawned += 1;
            }
            else
            {
                EnemySpawned = Instantiate(Enemies[NormalEnemyIndex], SpawnPosition + PositionOnCircumference, Quaternion.identity, transform);
                NumberOfEnemiesSpawned += 1;
            }
            if (SettlementToGuard)
            {
                EnemySpawned.GetComponent<EnemyController>().GuardSettlement(SettlementToGuard); //set them to guard the settlement
            }
        }

    }

    Settlement GetSettlementOutOfCameraBounds()
    {
        //Get Random settlement that isn't destroyed and is outside the camera bounds
        if (WorldGenerator.UndestroyedSettlements.Count <= 0) return null; //if no settlements, return null
        int RandomSettlement = Random.Range(0, WorldGenerator.UndestroyedSettlements.Count-1);
        Settlement Settlement = WorldGenerator.UndestroyedSettlements[RandomSettlement];
        while(PositionInCameraBounds(Settlement.transform.position) || Vector3.Distance(Player.transform.position, Settlement.transform.position) > 200)
        {
            RandomSettlement = Random.Range(0, WorldGenerator.UndestroyedSettlements.Count - 1);
            Settlement = WorldGenerator.UndestroyedSettlements[RandomSettlement];
        }

        return Settlement;
    }

    bool PositionInCameraBounds(Vector3 Position)
    {
        // Convert object's position to viewport space
        Vector3 ViewportPos = MainCamera.WorldToViewportPoint(Position);

        // Check if object is within camera bounds
        if (ViewportPos.x >= 0 && ViewportPos.x <= 1 &&
            ViewportPos.y >= 0 && ViewportPos.y <= 1 &&
            ViewportPos.z > 0) // Ensure the object is in front of the camera
        {
            return true;
        }
        return false;
    }

    Vector3 GetSpawnPoint()
    {
        Vector3 RadiusCenter = Player.transform.position;
        //Randomly select point a set distance away
        Vector3 Offset = new Vector3(Random.Range(-SpawnRadiusSize, SpawnRadiusSize), Random.Range(-SpawnRadiusSize, SpawnRadiusSize));
        Vector3 SpawnPoint = RadiusCenter + Offset;
        return SpawnPoint;
    }
}
