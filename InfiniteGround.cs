using UnityEngine;
using System.Collections.Generic;

public class InfiniteGround : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject groundTilePrefab;
    public int tilesAhead = 10;
    public int tilesBehind = 5;
    public float tileLength = 10f;
    
    [Header("Références")]
    public Transform player;
    
    private List<GameObject> activeTiles = new List<GameObject>();
    private float lastTileZ = 0f;

    void Start()
    {
        // Vérifications de sécurité
        if (groundTilePrefab == null)
        {
            Debug.LogError("ERREUR: groundTilePrefab n'est pas assigné dans InfiniteGround !");
            Debug.LogWarning("Veuillez assigner un prefab de sol dans l'inspecteur ou désactiver ce script.");
            enabled = false;
            return;
        }
        
        // Trouver le joueur automatiquement
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
        
        // Générer les premières tuiles
        for (int i = -tilesBehind; i < tilesAhead; i++)
        {
            SpawnTile(i * tileLength);
        }
        
        lastTileZ = (tilesAhead - 1) * tileLength;
    }

    void Update()
    {
        if (player == null) return;
        
        // Générer de nouvelles tuiles devant le joueur
        while (lastTileZ < player.position.z + tilesAhead * tileLength)
        {
            lastTileZ += tileLength;
            SpawnTile(lastTileZ);
        }
        
       
        CleanupOldTiles();
    }
    
    void SpawnTile(float zPosition)
    {
        if (groundTilePrefab == null) return;
        
        Vector3 spawnPos = new Vector3(0, 0, zPosition);
        GameObject tile = Instantiate(groundTilePrefab, spawnPos, Quaternion.identity);
        tile.transform.SetParent(transform);
        activeTiles.Add(tile);
    }
    
    void CleanupOldTiles()
    {
        for (int i = activeTiles.Count - 1; i >= 0; i--)
        {
            if (activeTiles[i] == null)
            {
                activeTiles.RemoveAt(i);
                continue;
            }
            
            if (activeTiles[i].transform.position.z < player.position.z - tilesBehind * tileLength)
            {
                Destroy(activeTiles[i]);
                activeTiles.RemoveAt(i);
            }
        }
    }
}