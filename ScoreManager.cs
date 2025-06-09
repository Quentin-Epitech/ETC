using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score UI")]
    public Text scoreText; // Texte pour afficher le score
    public Text bestScoreText; // Texte pour afficher le meilleur score
    
    [Header("Score Settings")]
    public int pointsPerSecond = 10; // Points gagnés par seconde
    public int obstaclePoints = 50; // Points bonus pour éviter un obstacle
    
    private int currentScore = 0;
    private int bestScore = 0;
    private float timeAlive = 0f;
    private bool gameActive = true;
    
    void Start()
    {
        // Charger le meilleur score sauvegardé
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateScoreUI();
        UpdateBestScoreUI();
    }
    
    void Update()
    {
        if (gameActive)
        {
            // Augmenter le score avec le temps
            timeAlive += Time.deltaTime;
            currentScore = Mathf.FloorToInt(timeAlive * pointsPerSecond);
            UpdateScoreUI();
        }
    }
    
    // Ajouter des points bonus (par exemple pour éviter un obstacle)
    public void AddBonusPoints(int points)
    {
        if (gameActive)
        {
            currentScore += points;
            UpdateScoreUI();
        }
    }
    
    // Appeler quand le joueur évite un obstacle
    public void ObstacleAvoided()
    {
        AddBonusPoints(obstaclePoints);
        Debug.Log("Obstacle évité ! +" + obstaclePoints + " points");
    }
    
    // Appeler à la fin du jeu
    public void GameOver()
    {
        gameActive = false;
        
        // Vérifier si c'est un nouveau record
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
            Debug.Log("Nouveau record ! Score : " + bestScore);
        }
        
        UpdateBestScoreUI();
    }
    
    // Redémarrer le score
    public void RestartScore()
    {
        currentScore = 0;
        timeAlive = 0f;
        gameActive = true;
        UpdateScoreUI();
    }
    
    // Mettre à jour l'affichage du score
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
    
    // Mettre à jour l'affichage du meilleur score
    private void UpdateBestScoreUI()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = "Record: " + bestScore.ToString();
        }
    }
    
    // Getters publics
    public int GetCurrentScore() { return currentScore; }
    public int GetBestScore() { return bestScore; }
    public bool IsGameActive() { return gameActive; }
}