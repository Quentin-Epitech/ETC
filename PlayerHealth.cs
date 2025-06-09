using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie du joueur")]
    public int maxLives = 3;
    private int currentLives;
    
    [Header("UI - Cœurs")]
    public Texture2D hearthTexture; // Votre asset "Hearth" (en Texture)
    public Transform heartsContainer; // Parent pour organiser les cœurs (Panel horizontal)
    private List<GameObject> heartsList = new List<GameObject>();
    
    [Header("UI - Autres")]
    public GameObject gameOverPanel; // Panel de fin de jeu
    
    [Header("Invincibilité temporaire")]
    public float invincibilityDuration = 2f;
    public float startInvincibilityDuration = 3f; // Invincibilité au démarrage
    private bool isInvincible = false;
    
    [Header("Score")]
    public ScoreManager scoreManager; // Référence au ScoreManager
    
    [Header("Effets visuels")]
    public GameObject damageEffect; // Effet visuel lors des dégâts
    private Renderer playerRenderer;
    private Animator playerAnimator; // Pour contrôler les animations
    private Material originalMaterial; // Matériau original
    private Color originalColor; // Couleur originale
    
    void Start()
    {
        currentLives = maxLives;
        playerRenderer = GetComponentInChildren<Renderer>(); // Chercher dans les enfants aussi
        playerAnimator = GetComponentInChildren<Animator>(); // Récupérer l'Animator
        
        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
            originalColor = playerRenderer.material.color;
        }
        
        CreateHeartsUI();
        
        // Démarrer l'invincibilité de début de partie
        StartCoroutine(StartInvincibilityCoroutine());
    }
    
    private void CreateHeartsUI()
    {
        // Supprimer les anciens cœurs s'ils existent
        foreach (GameObject heart in heartsList)
        {
            if (heart != null)
                Destroy(heart);
        }
        heartsList.Clear();
        
        // Créer les nouveaux cœurs avec votre asset "Hearth"
        for (int i = 0; i < maxLives; i++)
        {
            // Créer un nouveau GameObject pour le cœur
            GameObject newHeart = new GameObject("Heart_" + i);
            newHeart.transform.SetParent(heartsContainer);
            
            // Ajouter le composant Image
            Image heartImage = newHeart.AddComponent<Image>();
            
            // Convertir la Texture en Sprite
            Sprite heartSprite = Sprite.Create(hearthTexture, 
                new Rect(0, 0, hearthTexture.width, hearthTexture.height), 
                new Vector2(0.5f, 0.5f));
            heartImage.sprite = heartSprite;
            
            // Ajuster la taille du cœur (2 fois plus petit)
            RectTransform rectTransform = newHeart.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(25, 25); // Taille réduite
            rectTransform.localScale = Vector3.one;
            
            heartsList.Add(newHeart);
        }
        
        UpdateHeartsUI();
    }
    
    public void TakeDamage()
    {
        if (isInvincible) return;
        
        currentLives--;
        Debug.Log("Vie perdue ! Vies restantes : " + currentLives);
        
        // Jouer l'animation de dégâts ou effet de recul
        if (playerAnimator != null)
        {
            // Option 1 : Si vous avez une animation de dégâts
            playerAnimator.SetTrigger("TakeDamage");
            
            // Option 2 : Ou créer un effet de "recul" simple
            StartCoroutine(DamageRecoilEffect());
        }
        
        // Activer l'invincibilité temporaire
        StartCoroutine(InvincibilityCoroutine());
        
        // Effet visuel de dégâts
        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        UpdateHeartsUI();
        
        // Vérifier si le joueur est mort
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartsList.Count; i++)
        {
            if (heartsList[i] != null)
            {
                // Afficher le cœur si le joueur a encore cette vie
                // Cacher le cœur si le joueur a perdu cette vie
                heartsList[i].SetActive(i < currentLives);
            }
        }
    }
    
    private System.Collections.IEnumerator StartInvincibilityCoroutine()
    {
        isInvincible = true;
        Debug.Log("Invincibilité de démarrage activée pour " + startInvincibilityDuration + " secondes");
        
        // Juste attendre sans effet visuel
        yield return new WaitForSeconds(startInvincibilityDuration);
        
        isInvincible = false;
        Debug.Log("Invincibilité de démarrage terminée");
    }
    
    private System.Collections.IEnumerator DamageRecoilEffect()
    {
        Vector3 originalScale = transform.localScale;
        
        // Effet de "compression" rapide
        transform.localScale = originalScale * 0.8f;
        yield return new WaitForSeconds(0.1f);
        
        // Retour à la normale
        transform.localScale = originalScale;
    }
    
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        // Effet de changement de couleur
        for (float i = 0; i < invincibilityDuration; i += 0.1f)
        {
            if (playerRenderer != null)
            {
                // Alterner entre couleur normale et rouge
                playerRenderer.material.color = (playerRenderer.material.color == originalColor) ? Color.red : originalColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        // S'assurer que le joueur a sa couleur normale à la fin
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
        
        isInvincible = false;
    }
    
    private void GameOver()
    {
        Debug.Log("Game Over !");
        Time.timeScale = 0f; // Pause le jeu
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        // Redémarrer le score
        if (scoreManager != null)
        {
            scoreManager.RestartScore();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    // Getters publics
    public int GetCurrentLives() { return currentLives; }
    public bool IsInvincible() { return isInvincible; }
}