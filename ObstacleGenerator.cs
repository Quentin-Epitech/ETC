using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Configuration des Obstacles")]
    public GameObject[] obstaclePrefabs;        // Tes obstacles depuis les assets
    
    [Header("Génération")]
    public float spawnDistance = 50f;           // Distance devant le joueur où spawner
    public float minObstacleSpacing = 15f;      // Distance minimum entre obstacles
    public float maxObstacleSpacing = 25f;      // Distance maximum entre obstacles
    public float destroyDistance = 30f;         // Distance derrière le joueur pour détruire
    
    [Header("Probabilités")]
    [Range(0f, 1f)]
    public float spawnChance = 0.4f;            // Chance de spawner un obstacle
    [Range(0f, 1f)]
    public float doubleLaneChance = 0.15f;      // Chance d'avoir 2 obstacles simultanés
    
    [Header("Références")]
    public Transform player;                    // Référence au joueur
    
    // Variables privées
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnZ = 0f;              // Position Z du prochain spawn
    private float[] lanePositions = { -3f, 0f, 3f }; // Positions des 3 voies
    
    void Start()
    {
        // Trouver le joueur automatiquement si pas assigné
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
        
        // Vérifier qu'on a des prefabs d'obstacles
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogError("ERREUR: Aucun prefab d'obstacle assigné!");
            enabled = false;
            return;
        }
        
        // Position initiale de spawn
        nextSpawnZ = player.position.z + spawnDistance;
        
        // Générer quelques obstacles au début
        for (int i = 0; i < 3; i++)
        {
            TrySpawnObstacle();
        }
    }
    
    void Update()
    {
        // Générer de nouveaux obstacles si nécessaire
        if (player.position.z + spawnDistance > nextSpawnZ)
        {
            TrySpawnObstacle();
        }
        
        // Nettoyer les anciens obstacles
        CleanupOldObstacles();
    }
    
    void TrySpawnObstacle()
    {
        // Calculer la prochaine position de spawn
        float spacing = Random.Range(minObstacleSpacing, maxObstacleSpacing);
        nextSpawnZ += spacing;
        
        // Vérifier si on doit spawner un obstacle
        if (Random.Range(0f, 1f) > spawnChance)
        {
            return; // Pas d'obstacle cette fois
        }
        
        // Déterminer quelles voies utiliser
        List<int> lanesToUse = new List<int>();
        
        // Décider si on fait un ou deux obstacles
        if (Random.Range(0f, 1f) < doubleLaneChance)
        {
            // Deux obstacles - mais JAMAIS sur les 3 voies
            int firstLane = Random.Range(0, 3);
            lanesToUse.Add(firstLane);
            
            // Choisir une deuxième voie différente
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
            // Un seul obstacle
            lanesToUse.Add(Random.Range(0, 3));
        }
        
        // Spawner les obstacles
        foreach (int lane in lanesToUse)
        {
            SpawnObstacleInLane(lane, nextSpawnZ);
        }
        
        // Debug pour vérifier
        Debug.Log($"Obstacles spawnés sur voie(s): {string.Join(", ", lanesToUse)} à Z: {nextSpawnZ}");
    }
    
    void SpawnObstacleInLane(int laneIndex, float zPosition)
    {
        // Choisir un obstacle aléatoire
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        // Position de spawn
        Vector3 spawnPos = new Vector3(lanePositions[laneIndex], 0, zPosition);
        
        // Créer l'obstacle
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        obstacle.transform.SetParent(transform);
        
        // S'assurer qu'il a le bon tag
        if (obstacle.tag == "Untagged")
        {
            obstacle.tag = "Obstacle";
        }
        
        // Ajouter un collider si nécessaire
        if (obstacle.GetComponent<Collider>() == null)
        {
            BoxCollider boxCollider = obstacle.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }
        
        // Ajouter le script de collision si nécessaire
        if (obstacle.GetComponent<ObstacleCollision>() == null)
        {
            obstacle.AddComponent<ObstacleCollision>();
        }
        
        // Ajouter à la liste des obstacles actifs
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
            
            // Détruire les obstacles trop loin derrière le joueur
            if (activeObstacles[i].transform.position.z < player.position.z - destroyDistance)
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
    }
    
    // Fonction pour ajuster la difficulté
    public void AdjustDifficulty(float difficultyMultiplier)
    {
        spawnChance = Mathf.Clamp(0.4f * difficultyMultiplier, 0.3f, 0.7f);
        doubleLaneChance = Mathf.Clamp(0.15f * difficultyMultiplier, 0.1f, 0.3f);
        minObstacleSpacing = Mathf.Clamp(15f / difficultyMultiplier, 8f, 20f);
        maxObstacleSpacing = Mathf.Clamp(25f / difficultyMultiplier, 12f, 30f);
    }
    
    // Méthodes de test
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
    
    // Visualisation dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        
        // Zone de spawn (vert)
        Gizmos.color = Color.green;
        Vector3 spawnZone = player.position + Vector3.forward * spawnDistance;
        Gizmos.DrawWireCube(spawnZone, new Vector3(10f, 2f, 5f));
        
        // Zone de destruction (rouge)
        Gizmos.color = Color.red;
        Vector3 destroyZone = player.position - Vector3.forward * destroyDistance;
        Gizmos.DrawWireCube(destroyZone, new Vector3(10f, 2f, 5f));
        
        // Positions des voies (jaune)
        Gizmos.color = Color.yellow;
        for (int i = 0; i < 3; i++)
        {
            Vector3 lanePos = new Vector3(lanePositions[i], 1f, player.position.z);
            Gizmos.DrawWireSphere(lanePos, 0.5f);
        }
        
        // Prochaine position de spawn (bleu)
        Gizmos.color = Color.blue;
        Vector3 nextSpawn = new Vector3(0, 1f, nextSpawnZ);
        Gizmos.DrawWireCube(nextSpawn, new Vector3(10f, 0.2f, 2f));
    }
}