using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // для легкого доступа
    
    [Header("Score Settings")]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int bestScore = 0;

    void Awake()
    {
        // только один ScoreManager в игре
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // сохраняем между сценами
            LoadBestScore();
        }
        else
        {
            Destroy(gameObject); // уничтожаем дубликаты
        }
    }

    // добавляем очки
    public void AddScore(int points)
    {
        currentScore += points;
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            SaveBestScore();
        }
    }
    
    // сбрасываем текущий счет при новой игре
    public void ResetCurrentScore()
    {
        currentScore = 0;
    }

    // сохраняем рекорд в PlayerPrefs
    private void SaveBestScore()
    {
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }

    // загружаем рекорд из PlayerPrefs
    private void LoadBestScore()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0); // 0 по умолчанию
    }
    
    // геттеры для получения значений
    public int GetCurrentScore() => currentScore;
    public int GetBestScore() => bestScore;
}
