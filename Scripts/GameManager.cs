using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject tilePrefab;
    private List<GameObject> _tiles;
    
    private int[,] _gameArea = new int[4,4]; // значения плиток
    private Vector2[,] _positions = new Vector2[4,4]; // позиция для UI

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
        _tiles = new List<GameObject>();
        _gameArea = new int[4,4];
        InitializeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.D)) MoveRight();
        if (Input.GetKeyDown(KeyCode.W)) MoveUp();
        if (Input.GetKeyDown(KeyCode.S)) MoveDown();
    }
    
    private void InitializeGame()
    {
        GenerateRandomTile();
        GenerateRandomTile();
    }

    public void RestartGame()
    {
        // очищаем поле
        _gameArea = new int[4, 4];
        
        // удаляем визуальные объекты
        foreach (GameObject tile in _tiles)
        {
            if(tile != null)
                DestroyImmediate(tile);
        }
        _tiles.Clear();
        
        // сбрасываем счет
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
        }
        
        // начинаем заново
        InitializeGame();
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

    public void MoveLeft()
    {
        bool moved = false;
        bool[,] merged = new bool[4, 4]; // для отслеживания какие плитки уже объединились в этом ходу

        for (int row = 0; row < 4; row++)
        {
            for (int col = 1; col < 4; col++) // начиная с первой колонны, двигаемся влево
            {
                if (_gameArea[row, col] != 0) // если плитка не пустая
                {
                    int currentCol = col;
                    
                    // двигаем плитку влево пока возможно
                    while (currentCol > 0)
                    {   
                        // если слева пусто - двигаем
                        if (_gameArea[row, currentCol - 1] == 0)
                        {
                            _gameArea[row, currentCol - 1] = _gameArea[row, currentCol];
                            _gameArea[row, currentCol] = 0;
                            currentCol--;
                            moved = true;
                        }
                        // если слева такая же плитка и она еще не объединялась
                        else if (_gameArea[row, currentCol - 1] == _gameArea[row, currentCol] &&
                                 !merged[row, currentCol - 1])
                        {
                            int mergedValue = _gameArea[row, currentCol - 1] * 2;
                            _gameArea[row, currentCol - 1] = mergedValue; // удваиваем значение (объединяем)
                            _gameArea[row, currentCol] = 0; // убираем исходную плитку
                            merged[row, currentCol - 1] = true; // помечаем как объединенную
                            moved = true;
                            
                            // добавляем очки
                            if (ScoreManager.Instance != null)
                            {
                                ScoreManager.Instance.AddScore(mergedValue);
                            }
                            break;
                        }
                        else
                        {
                            break; // не можем двигаться дальше, останавливаем цикл
                        }
                    }
                }
            }
        }
        if (moved)
        {
            UpdateVisuals();
            GenerateRandomTile();
        }
    }
    
    public void MoveRight()
    {
        bool moved = false;
        bool[,] merged = new bool[4, 4];

        for (int row = 0; row < 4; row++)
        {
            for (int col = 2; col >= 0; col--) // начинаем справа, двигаемся вправо
            {
                if (_gameArea[row, col] != 0) // если плитка не пустая
                {
                    int currentCol = col;
                    
                    // двигаем плитку вправо пока возможно
                    while (currentCol < 3)
                    {   
                        // если справа пусто - двигаем
                        if (_gameArea[row, currentCol + 1] == 0)
                        {
                            _gameArea[row, currentCol + 1] = _gameArea[row, currentCol];
                            _gameArea[row, currentCol] = 0;
                            currentCol++;
                            moved = true;
                        }
                        // если справа такая же плитка и она еще не объединялась
                        else if (_gameArea[row, currentCol + 1] == _gameArea[row, currentCol] &&
                                 !merged[row, currentCol + 1])
                        {
                            int mergedValue = _gameArea[row, currentCol + 1] * 2;
                            _gameArea[row, currentCol + 1] = mergedValue; // удваиваем значение (объединяем)
                            _gameArea[row, currentCol] = 0; // убираем исходную плитку
                            merged[row, currentCol + 1] = true; // помечаем как объединенную
                            moved = true;
                            
                            // добавляем очки
                            if (ScoreManager.Instance != null)
                            {
                                ScoreManager.Instance.AddScore(mergedValue);
                            }
                            break;
                        }
                        else
                        {
                            break; // не можем двигаться дальше, останавливаем цикл
                        }
                    }
                }
            }
        }
        if (moved)
        {
            UpdateVisuals();
            GenerateRandomTile();
        }
    }
    
    public void MoveUp()
    {
        bool moved = false;
        bool[,] merged = new bool[4, 4];

        for (int col = 0; col < 4; col++)
        {
            for (int row = 1; row < 4; row++) // начинаем с ряда 1, двигаемся вверх
            {
                if (_gameArea[row, col] != 0) // если плитка не пустая
                {
                    int currentRow = row;
                    
                    // двигаем плитку вверх пока возможно
                    while (currentRow > 0)
                    {   
                        // если сверху пусто - двигаем
                        if (_gameArea[currentRow - 1, col] == 0)
                        {
                            _gameArea[currentRow - 1, col] = _gameArea[currentRow, col];
                            _gameArea[currentRow, col] = 0;
                            currentRow--;
                            moved = true;
                        }
                        // если сверху такая же плитка и она еще не объединялась
                        else if (_gameArea[currentRow - 1, col] == _gameArea[currentRow, col] &&
                                 !merged[currentRow - 1, col])
                        {
                            int mergedValue = _gameArea[currentRow - 1, col] * 2;
                            _gameArea[currentRow - 1, col] = mergedValue; // удваиваем значение (объединяем)
                            _gameArea[currentRow, col] = 0; // убираем исходную плитку
                            merged[currentRow - 1, col] = true; // помечаем как объединенную
                            moved = true;
                            
                            // добавляем очки
                            if (ScoreManager.Instance != null)
                            {
                                ScoreManager.Instance.AddScore(mergedValue);
                            }
                            break;
                        }
                        else
                        {
                            break; // не можем двигаться дальше, останавливаем цикл
                        }
                    }
                }
            }
        }
        if (moved)
        {
            UpdateVisuals();
            GenerateRandomTile();
        }
    }
    
    public void MoveDown()
    {
        bool moved = false;
        bool[,] merged = new bool[4, 4];

        for (int col = 0; col < 4; col++)
        {
            for (int row = 2; row >= 0; row--) // начинаем снизу, двигаемся вниз
            {
                if (_gameArea[row, col] != 0) // если плитка не пустая
                {
                    int currentRow = row;
                    
                    // двигаем плитку вниз пока возможно
                    while (currentRow < 3)
                    {   
                        // если снизу пусто - двигаем
                        if (_gameArea[currentRow + 1, col] == 0)
                        {
                            _gameArea[currentRow + 1, col] = _gameArea[currentRow, col];
                            _gameArea[currentRow, col] = 0;
                            currentRow++;
                            moved = true;
                        }
                        // если снизу такая же плитка и она еще не объединялась
                        else if (_gameArea[currentRow + 1, col] == _gameArea[currentRow, col] &&
                                 !merged[currentRow + 1, col])
                        {
                            int mergedValue = _gameArea[currentRow + 1, col] * 2;
                            _gameArea[currentRow + 1, col] = mergedValue; // удваиваем значение (объединяем)
                            _gameArea[currentRow, col] = 0; // убираем исходную плитку
                            merged[currentRow + 1, col] = true; // помечаем как объединенную
                            moved = true;
                            
                            // добавляем очки
                            if (ScoreManager.Instance != null)
                            {
                                ScoreManager.Instance.AddScore(mergedValue);
                            }
                            break;
                        }
                        else
                        {
                            break; // не можем двигаться дальше, останавливаем цикл
                        }
                    }
                }
            }
        }
        if (moved)
        {
            UpdateVisuals();
            GenerateRandomTile();
        }
    }

    private void UpdateVisuals()
    {
        // удаляем все существующие визуальные плитки
        foreach (GameObject tile in _tiles)
        {
            if (tile != null)
                DestroyImmediate(tile);
        }

        _tiles.Clear();

        // создаем новые объекты для всех НЕпустых клеток
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (_gameArea[row, col] != 0)
                {
                    GenerateTileVisual(new Vector2Int(row, col), _gameArea[row, col]);
                }
            }
        }
    }
}
