using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score UI")]
    public Text scoreText; 
    public Text bestScoreText; 
    
    [Header("Score Settings")]
    public int pointsPerSecond = 10; 
    public int obstaclePoints = 50; 
    
    private int currentScore = 0;
    private int bestScore = 0;
    private float timeAlive = 0f;
    private bool gameActive = true;
    
    void Start()
    {
        
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateScoreUI();
        UpdateBestScoreUI();
    }
    
    void Update()
    {
        if (gameActive)
        {
            
            timeAlive += Time.deltaTime;
            currentScore = Mathf.FloorToInt(timeAlive * pointsPerSecond);
            UpdateScoreUI();
        }
    }
    
    
    public void AddBonusPoints(int points)
    {
        if (gameActive)
        {
            currentScore += points;
            UpdateScoreUI();
        }
    }
    
    
    public void ObstacleAvoided()
    {
        AddBonusPoints(obstaclePoints);
        Debug.Log("Obstacle évité ! +" + obstaclePoints + " points");
    }
    
    
    public void GameOver()
    {
        gameActive = false;
        
        
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
            Debug.Log("Nouveau record ! Score : " + bestScore);
        }
        
        UpdateBestScoreUI();
    }
    
   
    public void RestartScore()
    {
        currentScore = 0;
        timeAlive = 0f;
        gameActive = true;
        UpdateScoreUI();
    }
    
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
    
    
    private void UpdateBestScoreUI()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = "Record: " + bestScore.ToString();
        }
    }
    
   
    public int GetCurrentScore() { return currentScore; }
    public int GetBestScore() { return bestScore; }
    public bool IsGameActive() { return gameActive; }
}