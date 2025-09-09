using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    private List<GameObject> _tiles;
    
    private int[,] _gameArea = new int[4,4]; // значения плиток
    private Vector2[,] _positions = new Vector2[4,4]; // позиция для UI
    
    // Start is called before the first frame update
    void Start()
    {
        _tiles = new List<GameObject>();
        _gameArea = new int[4,4];
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        GenerateRandomTile();
        GenerateRandomTile();
    }

    private Vector2Int GetRandomPosition()
    {
        int randomRow = Random.Range(0, _gameArea.GetLength(0));
        int randomCol = Random.Range(0, _gameArea.GetLength(1));
        if (_gameArea[randomRow, randomCol] == 0)
        {
            return new Vector2Int(randomRow, randomCol);
        }
        else
        {
            return GetRandomPosition();
        }
    }
    
    private void GenerateRandomTile()
    {
        // Получаем случайную пустую позицию
        Vector2Int randomPosition = GetRandomPosition();
        // Определяем значение новой плитки (90% - двойка, 10% - четверка)
        int newTileValue = Random.Range(0f, 1f) < 0.9f ? 2 : 4;
        // Обновляем игровое поле
        _gameArea[randomPosition.x, randomPosition.y] = newTileValue;
        // Создаем визуальный объект
        GenerateTileVisual(randomPosition, newTileValue);
    }

    private void GenerateTileVisual(Vector2Int gridPos, int value)
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        GameObject tile = Instantiate(tilePrefab, canvas.transform);

        if (_tiles == null)
            _tiles = new List<GameObject>();
        _tiles.Add(tile);
        
        // Устанавливаем позицию
        RectTransform rectTransform = tile.GetComponent<RectTransform>();
        Vector2 worldPos = GridToWorldPosition(gridPos);
        rectTransform.anchoredPosition = worldPos;
        
        var textComponent = tile.GetComponentInChildren<UnityEngine.UI.Text>();
        if (textComponent != null)
        {
            textComponent.text = value.ToString();
        }
    }

    private Vector2 GridToWorldPosition(Vector2Int gridPos)
    {
        // Сетка 4х4 с центром в (0, 0) и отступ 300 единиц
        float cellSize = 300f;
        float startX = -1.5f * cellSize;
        float startY = 1.5f * cellSize;
        
        float x = startX + gridPos.y * cellSize;
        float y = startY - gridPos.x * cellSize;
        
        return new Vector2(x, y);
    }
}
