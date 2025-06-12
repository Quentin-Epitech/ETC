using UnityEngine;

public class FireEnvironmentSpawner : MonoBehaviour
{
    [Header("Génération automatique de décor FEU")]
    public Transform player; 
    
    [Header("Synchronisation avec InfiniteGround")]
    public InfiniteGround infiniteGround; 
    public bool syncWithGround = true; 
    
    [Header("Configuration")]
    public float spawnDistance = 40f; 
    public float despawnDistance = 60f; 
    public float minSideDistance = 10f; 
    public float maxSideDistance = 30f; 
    public float spawnInterval = 10f; 
    
    [Header("Densité du décor")]
    [Range(0f, 1f)] public float volcanoRockChance = 0.4f;
    [Range(0f, 1f)] public float fireCrystalChance = 0.3f;
    [Range(0f, 1f)] public float torchChance = 0.2f;
    [Range(0f, 1f)] public float geyserChance = 0.1f;
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    private float lastSpawnZ = -999f;
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
            {
                Debug.LogError("Player non trouvé ! Assurez-vous que votre joueur a le tag 'Player'");
                return;
            }
        }
        
        
        if (infiniteGround == null && syncWithGround)
        {
            infiniteGround = FindObjectOfType<InfiniteGround>();
            if (infiniteGround != null)
            {
                
                spawnInterval = infiniteGround.tileLength;
                if (showDebugMessages)
                    Debug.Log($"Synchronisé avec InfiniteGround. Interval: {spawnInterval}");
            }
        }
        
        
        Invoke("DelayedStart", 0.1f);
    }
    
    void DelayedStart()
    {
        
        lastSpawnZ = player.position.z - 100f;
        
        if (showDebugMessages)
            Debug.Log($"FireEnvironmentSpawner initialisé. Position joueur : {player.position.z}");
        
        
        for (int i = -5; i < 15; i++)
        {
            float spawnZ = player.position.z + (i * spawnInterval);
            SpawnFireEnvironmentChunk(spawnZ);
        }
        
        
        lastSpawnZ = player.position.z + (14 * spawnInterval);
    }
    
    void Update()
    {
        if (player == null) return;
        
        float playerZ = player.position.z;
        
        
        if (syncWithGround && infiniteGround != null)
        {
            
            int tilesAhead = infiniteGround.tilesAhead;
            float tileLength = infiniteGround.tileLength;
            
            
            while (lastSpawnZ < playerZ + tilesAhead * tileLength)
            {
                lastSpawnZ += tileLength;
                
                if (showDebugMessages)
                    Debug.Log($"SYNC SPAWN: Décor à Z: {lastSpawnZ} (Joueur à Z: {playerZ:F1})");
                
                SpawnFireEnvironmentChunk(lastSpawnZ);
            }
        }
        else
        {
            
            while (lastSpawnZ < playerZ + spawnDistance)
            {
                lastSpawnZ += spawnInterval;
                
                if (showDebugMessages)
                    Debug.Log($"AUTO SPAWN: Décor à Z: {lastSpawnZ} (Joueur à Z: {playerZ:F1})");
                
                SpawnFireEnvironmentChunk(lastSpawnZ);
            }
        }
        
       
        if (Time.frameCount % 300 == 0) 
        {
            if (syncWithGround && infiniteGround != null)
            {
                float cleanupDistance = infiniteGround.tilesBehind * infiniteGround.tileLength;
                CleanupBehindPlayer(playerZ - cleanupDistance);
            }
            else
            {
                CleanupBehindPlayer(playerZ - despawnDistance);
            }
            
            if (showDebugMessages)
                Debug.Log($"Position joueur: {playerZ:F1}, Dernier spawn: {lastSpawnZ:F1}");
        }
    }
    
    void SpawnFireEnvironmentChunk(float zPosition)
    {
        if (showDebugMessages)
            Debug.Log($"=== Génération chunk à Z: {zPosition:F1} ===");
        
        int objectsPerSide = 3; 
        
        
        for (int i = 0; i < objectsPerSide; i++)
        {
            float leftX = Random.Range(-maxSideDistance, -minSideDistance);
            float leftZ = zPosition + Random.Range(-spawnInterval * 0.3f, spawnInterval * 0.3f);
            
            SpawnRandomFireObject(leftX, leftZ);
        }
        
        
        for (int i = 0; i < objectsPerSide; i++)
        {
            float rightX = Random.Range(minSideDistance, maxSideDistance);
            float rightZ = zPosition + Random.Range(-spawnInterval * 0.3f, spawnInterval * 0.3f);
            
            SpawnRandomFireObject(rightX, rightZ);
        }
        
       
        if (Random.value < 0.3f)
        {
            SpawnBackgroundFire(zPosition);
        }
    }
    
    void SpawnRandomFireObject(float x, float z)
    {
        
        if (Mathf.Abs(x) < minSideDistance)
        {
            if (showDebugMessages)
                Debug.LogWarning($"SÉCURITÉ: Objet trop proche (X: {x}), repositionnement");
            
            x = (x < 0) ? -minSideDistance - 2f : minSideDistance + 2f;
        }
        
        float random = Random.value;
        
        if (random < volcanoRockChance)
        {
            CreateVolcanoRock(x, z);
        }
        else if (random < volcanoRockChance + fireCrystalChance)
        {
            CreateFireCrystal(x, z);
        }
        else if (random < volcanoRockChance + fireCrystalChance + torchChance)
        {
            CreateFireTorch(x, z);
        }
        else if (random < volcanoRockChance + fireCrystalChance + torchChance + geyserChance)
        {
            CreateFireGeyser(x, z);
        }
        else
        {
            
            CreateVolcanoRock(x, z);
        }
    }
    
    void CreateVolcanoRock(float x, float z)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.transform.position = new Vector3(x, Random.Range(0.3f, 1.2f), z);
        rock.transform.localScale = new Vector3(
            Random.Range(0.8f, 2f), 
            Random.Range(0.6f, 1.5f), 
            Random.Range(0.8f, 2f)
        );
        rock.transform.rotation = Quaternion.Euler(
            Random.Range(-8f, 8f), 
            Random.Range(0f, 360f), 
            Random.Range(-8f, 8f)
        );
        
        Destroy(rock.GetComponent<Collider>());
        rock.GetComponent<Renderer>().material.color = new Color(0.25f, 0.1f, 0.08f);
        rock.tag = "EnvironmentDecor";
        rock.name = "VolcanoRock";
    }
    
    void CreateFireCrystal(float x, float z)
    {
        GameObject crystal = new GameObject("FireCrystal");
        
        for (int i = 0; i < 3; i++)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.transform.SetParent(crystal.transform);
            part.transform.localPosition = new Vector3(0, i * 0.5f, 0);
            part.transform.localScale = new Vector3(0.4f - i * 0.08f, 0.7f, 0.4f - i * 0.08f);
            
            Destroy(part.GetComponent<Collider>());
            part.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.1f);
        }
        
        crystal.transform.position = new Vector3(x, 0, z);
        crystal.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        crystal.tag = "EnvironmentDecor";
    }
    
    void CreateFireTorch(float x, float z)
    {
        GameObject torch = new GameObject("FireTorch");
        
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(torch.transform);
        pole.transform.localPosition = new Vector3(0, 1.2f, 0);
        pole.transform.localScale = new Vector3(0.12f, 1.2f, 0.12f);
        Destroy(pole.GetComponent<Collider>());
        pole.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f);
        
        GameObject flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flame.transform.SetParent(torch.transform);
        flame.transform.localPosition = new Vector3(0, 2.6f, 0);
        flame.transform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
        Destroy(flame.GetComponent<Collider>());
        flame.GetComponent<Renderer>().material.color = new Color(1f, 0.9f, 0.3f);
        
        torch.transform.position = new Vector3(x, 0, z);
        torch.tag = "EnvironmentDecor";
    }
    
    void CreateFireGeyser(float x, float z)
    {
        GameObject geyser = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        geyser.transform.position = new Vector3(x, 1.5f, z);
        geyser.transform.localScale = new Vector3(0.6f, 3f, 0.6f);
        Destroy(geyser.GetComponent<Collider>());
        geyser.GetComponent<Renderer>().material.color = new Color(0.8f, 0.15f, 0.05f);
        geyser.tag = "EnvironmentDecor";
        geyser.name = "FireGeyser";
    }
    
    void SpawnBackgroundFire(float zPosition)
    {
        GameObject mountain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        float mountainX = (Random.value < 0.5f) ? 
            Random.Range(-80f, -50f) : Random.Range(50f, 80f);
        
        mountain.transform.position = new Vector3(
            mountainX, 
            Random.Range(6f, 12f), 
            zPosition + Random.Range(5f, 25f)
        );
        mountain.transform.localScale = new Vector3(
            Random.Range(8f, 15f), 
            Random.Range(12f, 20f), 
            Random.Range(8f, 12f)
        );
        Destroy(mountain.GetComponent<Collider>());
        mountain.GetComponent<Renderer>().material.color = new Color(0.15f, 0.08f, 0.06f);
        mountain.tag = "EnvironmentDecor";
        mountain.name = "VolcanoMountain";
    }
    
    void CleanupBehindPlayer(float cleanupZ)
    {
        GameObject[] decorObjects = GameObject.FindGameObjectsWithTag("EnvironmentDecor");
        int cleaned = 0;
        
        foreach (GameObject obj in decorObjects)
        {
            if (obj != null && obj.transform.position.z < cleanupZ)
            {
                Destroy(obj);
                cleaned++;
            }
        }
        
        if (cleaned > 0 && showDebugMessages)
        {
            Debug.Log($"Nettoyé {cleaned} objets de décor derrière Z: {cleanupZ:F1}");
        }
    }
}