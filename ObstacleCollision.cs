using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Configuration")]
    public bool destroyOnHit = false; // Si l'obstacle doit être détruit après collision
    public GameObject hitEffect; // Effet visuel lors de la collision
    
    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est le joueur qui entre en collision
        if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other.gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Alternative avec OnCollisionEnter si vous utilisez des colliders solides
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
    }
    
    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Collision avec obstacle détectée !");
        
        // Récupérer le composant PlayerHealth
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
            // Faire perdre une vie au joueur
            playerHealth.TakeDamage();
            
            // Effet visuel de collision
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Optionnel : détruire l'obstacle après collision
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("PlayerHealth component non trouvé sur le joueur !");
        }
    }
}