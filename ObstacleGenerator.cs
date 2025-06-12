using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Configuration des Obstacles")]
    public GameObject[] obstaclePrefabs;        
    
    [Header("Génération")]
    public float spawnDistance = 50f;           
    public float minObstacleSpacing = 15f;     
    public float maxObstacleSpacing = 25f;      
    public float destroyDistance = 30f;         
    
    [Header("Probabilités")]
    [Range(0f, 1f)]
    public float spawnChance = 0.4f;           
    [Range(0f, 1f)]
    public float doubleLaneChance = 0.15f;      
    
    [Header("Références")]
    public Transform player;                    
    
    
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnZ = 0f;             
    private float[] lanePositions = { -3f, 0f, 3f }; 
    
    void Start()
    {
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("ERREUR: Joueur non trouvé! Assurez-vous que le joueur a le tag 'Player'");
                enabled = false;
                return;
            }
        }
        
       
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogError("ERREUR: Aucun prefab d'obstacle assigné!");
            enabled = false;
            return;
        }
        
      
        nextSpawnZ = player.position.z + spawnDistance;
        
        
        for (int i = 0; i < 3; i++)
        {
            TrySpawnObstacle();
        }
    }
    
    void Update()
    {
       
        if (player.position.z + spawnDistance > nextSpawnZ)
        {
            TrySpawnObstacle();
        }
        
       
        CleanupOldObstacles();
    }
    
    void TrySpawnObstacle()
    {
      
        float spacing = Random.Range(minObstacleSpacing, maxObstacleSpacing);
        nextSpawnZ += spacing;
        
       
        if (Random.Range(0f, 1f) > spawnChance)
        {
            return; 
        }
        
        
        List<int> lanesToUse = new List<int>();
        
       
        if (Random.Range(0f, 1f) < doubleLaneChance)
        {
            
            int firstLane = Random.Range(0, 3);
            lanesToUse.Add(firstLane);
            
           
            List<int> availableLanes = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (i != firstLane)
                {
                    availableLanes.Add(i);
                }
            }
            
            if (availableLanes.Count > 0)
            {
                int secondLane = availableLanes[Random.Range(0, availableLanes.Count)];
                lanesToUse.Add(secondLane);
            }
        }
        else
        {
           
            lanesToUse.Add(Random.Range(0, 3));
        }
        
        
        foreach (int lane in lanesToUse)
        {
            SpawnObstacleInLane(lane, nextSpawnZ);
        }
        
        
        Debug.Log($"Obstacles spawnés sur voie(s): {string.Join(", ", lanesToUse)} à Z: {nextSpawnZ}");
    }
    
    void SpawnObstacleInLane(int laneIndex, float zPosition)
    {
        
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        
        Vector3 spawnPos = new Vector3(lanePositions[laneIndex], 0, zPosition);
        
       
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        obstacle.transform.SetParent(transform);
        
        
        if (obstacle.tag == "Untagged")
        {
            obstacle.tag = "Obstacle";
        }
        
       
        if (obstacle.GetComponent<Collider>() == null)
        {
            BoxCollider boxCollider = obstacle.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }
        
      
        if (obstacle.GetComponent<ObstacleCollision>() == null)
        {
            obstacle.AddComponent<ObstacleCollision>();
        }
        
    
        activeObstacles.Add(obstacle);
    }
    
    void CleanupOldObstacles()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }
            
           
            if (activeObstacles[i].transform.position.z < player.position.z - destroyDistance)
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
    }
    
   
    public void AdjustDifficulty(float difficultyMultiplier)
    {
        spawnChance = Mathf.Clamp(0.4f * difficultyMultiplier, 0.3f, 0.7f);
        doubleLaneChance = Mathf.Clamp(0.15f * difficultyMultiplier, 0.1f, 0.3f);
        minObstacleSpacing = Mathf.Clamp(15f / difficultyMultiplier, 8f, 20f);
        maxObstacleSpacing = Mathf.Clamp(25f / difficultyMultiplier, 12f, 30f);
    }
    
    // test
    [ContextMenu("Générer Obstacle Test")]
    void GenerateTestObstacle()
    {
        TrySpawnObstacle();
    }
    
    [ContextMenu("Nettoyer Tous les Obstacles")]
    void ClearAllObstacles()
    {
        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                DestroyImmediate(obstacle);
            }
        }
        activeObstacles.Clear();
        nextSpawnZ = player.position.z + spawnDistance;
    }
    
   
    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        
       
        Gizmos.color = Color.green;
        Vector3 spawnZone = player.position + Vector3.forward * spawnDistance;
        Gizmos.DrawWireCube(spawnZone, new Vector3(10f, 2f, 5f));
        
        
        Gizmos.color = Color.red;
        Vector3 destroyZone = player.position - Vector3.forward * destroyDistance;
        Gizmos.DrawWireCube(destroyZone, new Vector3(10f, 2f, 5f));
        
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < 3; i++)
        {
            Vector3 lanePos = new Vector3(lanePositions[i], 1f, player.position.z);
            Gizmos.DrawWireSphere(lanePos, 0.5f);
        }
        
        
        Gizmos.color = Color.blue;
        Vector3 nextSpawn = new Vector3(0, 1f, nextSpawnZ);
        Gizmos.DrawWireCube(nextSpawn, new Vector3(10f, 0.2f, 2f));
    }
}