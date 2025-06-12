using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie du joueur")]
    public int maxLives = 3;
    private int currentLives;
    
    [Header("UI - Cœurs")]
    public Texture2D hearthTexture; 
    public Transform heartsContainer; 
    private List<GameObject> heartsList = new List<GameObject>();
    
    [Header("UI - Autres")]
    public GameObject gameOverPanel; 
    
    [Header("Invincibilité temporaire")]
    public float invincibilityDuration = 2f;
    public float startInvincibilityDuration = 3f; 
    private bool isInvincible = false;
    
    [Header("Score")]
    public ScoreManager scoreManager; 
    
    [Header("Effets visuels")]
    public GameObject damageEffect; 
    private Renderer playerRenderer;
    private Animator playerAnimator; 
    private Material originalMaterial; 
    private Color originalColor; 
    
    void Start()
    {
        currentLives = maxLives;
        playerRenderer = GetComponentInChildren<Renderer>(); 
        playerAnimator = GetComponentInChildren<Animator>(); 
        
        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
            originalColor = playerRenderer.material.color;
        }
        
        CreateHeartsUI();
        
        StartCoroutine(StartInvincibilityCoroutine());
    }
    
    private void CreateHeartsUI()
    {
       
        foreach (GameObject heart in heartsList)
        {
            if (heart != null)
                Destroy(heart);
        }
        heartsList.Clear();
        
        
        for (int i = 0; i < maxLives; i++)
        {
            
            GameObject newHeart = new GameObject("Heart_" + i);
            newHeart.transform.SetParent(heartsContainer);
            
            
            Image heartImage = newHeart.AddComponent<Image>();
            
            // Convertir la texture en Sprite
            Sprite heartSprite = Sprite.Create(hearthTexture, 
                new Rect(0, 0, hearthTexture.width, hearthTexture.height), 
                new Vector2(0.5f, 0.5f));
            heartImage.sprite = heartSprite;
            
            
            RectTransform rectTransform = newHeart.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(25, 25); 
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
        
        
        if (playerAnimator != null)
        {
            
            playerAnimator.SetTrigger("TakeDamage");
            
            
            StartCoroutine(DamageRecoilEffect());
        }
        
        
        StartCoroutine(InvincibilityCoroutine());
        
        
        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        UpdateHeartsUI();
        
        
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
                
                heartsList[i].SetActive(i < currentLives);
            }
        }
    }
    
    private System.Collections.IEnumerator StartInvincibilityCoroutine()
    {
        isInvincible = true;
        Debug.Log("Invincibilité de démarrage activée pour " + startInvincibilityDuration + " secondes");
        
       
        yield return new WaitForSeconds(startInvincibilityDuration);
        
        isInvincible = false;
        Debug.Log("Invincibilité de démarrage terminée");
    }
    
    private System.Collections.IEnumerator DamageRecoilEffect()
    {
        Vector3 originalScale = transform.localScale;
        
      
        transform.localScale = originalScale * 0.8f;
        yield return new WaitForSeconds(0.1f);
        
      
        transform.localScale = originalScale;
    }
    
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        
        for (float i = 0; i < invincibilityDuration; i += 0.1f)
        {
            if (playerRenderer != null)
            {
                
                playerRenderer.material.color = (playerRenderer.material.color == originalColor) ? Color.red : originalColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
        
        isInvincible = false;
    }
    
    private void GameOver()
    {
        Debug.Log("Game Over !");
        Time.timeScale = 0f; 
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        
        if (scoreManager != null)
        {
            scoreManager.RestartScore();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    
    public int GetCurrentLives() { return currentLives; }
    public bool IsInvincible() { return isInvincible; }
}