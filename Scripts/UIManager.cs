using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Score UI")]
    [SerializeField] private Text bestScoreText;
    [SerializeField] private Text scoreText;
    
    [Header("Game UI")]
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        UpdateScoreUI();
    }
    public void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        if (bestScoreText != null)
            bestScoreText.text = "Best: " + ScoreManager.Instance.GetBestScore();
    }

    public void HideAllPanels()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void RestartGame()
    {
        HideAllPanels();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
