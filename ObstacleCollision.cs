using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Configuration")]
    public bool destroyOnHit = false; 
    public GameObject hitEffect; 
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other.gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
    }
    
    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Collision avec obstacle détectée !");
        
        
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
           
            playerHealth.TakeDamage();
            
            
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            
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