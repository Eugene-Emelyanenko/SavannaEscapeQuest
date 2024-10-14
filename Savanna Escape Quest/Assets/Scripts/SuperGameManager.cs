using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SuperGameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform heartsContainer;
    private int currentHealth;
    public Vector2Int playerPosition;

    [Header("Grid")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform cellContainer;
    public int gridWidth;
    public int gridHeight;
    private Cell[][] grid;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image gameOverFrame;
    [SerializeField] private Sprite loseFrame;
    [SerializeField] private Sprite winFrame;
    [SerializeField] GameObject tryAgainButton;
    [SerializeField] private int coinsForLevel = 50;
    [SerializeField] private TextMeshProUGUI bunnyCountText;
    public int bunniesCount = 5;
    public int heartsCount = 5;
    public int elephantCount = 5;
    public int trapCount = 5;

    private Vector2Int previousPlayerPosition;
    private int collectedbunnies = 0;

    private void Start()
    {
        StartGame();
        gameOverPanel.SetActive(false);
    }

    private void StartGame()
    {
        bunnyCountText.text = $"{0}/{bunniesCount}";

        currentHealth = 3;
        UpdateHealth();

        InitializeGrid();
        PlacePlayer();
        PlaceObjectsRandomly();
    }

    private void InitializeGrid()
    {
        foreach (Transform t in cellContainer)
        {
            Destroy(t.gameObject);
        }

        grid = new Cell[gridHeight][];

        for (int y = 0; y < gridHeight; y++)
        {
            grid[y] = new Cell[gridWidth];          

            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cellObj = Instantiate(cellPrefab, cellContainer);
                cellObj.name = $"Cell[y:{y}][x:{x}]";
                Cell cell = cellObj.GetComponent<Cell>();

                cell.SetUp(CellType.Empty);

                grid[y][x] = cell;
            }
        }
    }

    private void PlacePlayer()
    {
        playerPosition = new Vector2Int(0, 0);
        grid[playerPosition.y][playerPosition.x].SetUp(CellType.Player);
    }

    private void PlaceObjectsRandomly()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        // Собираем все свободные позиции, куда можно разместить объекты
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[y][x].Type == CellType.Empty)
                {
                    availablePositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Перемешиваем список свободных позиций
        Shuffle(availablePositions);

        // Размещаем зайцев
        for (int i = 0; i < bunniesCount && availablePositions.Count > 0; i++)
        {
            Vector2Int pos = availablePositions[0];
            availablePositions.RemoveAt(0);
            grid[pos.y][pos.x].SetUp(CellType.Bunny);
        }

        // Размещаем слонов
        for (int i = 0; i < elephantCount && availablePositions.Count > 0; i++)
        {
            Vector2Int pos = availablePositions[0];
            availablePositions.RemoveAt(0);
            grid[pos.y][pos.x].SetUp(CellType.Enemy, enemyDamage: 1, enemyIcon: "Elephant"); // Здесь предположим, что слоны — это враги
        }

        // Размещаем ловушки
        for (int i = 0; i < trapCount && availablePositions.Count > 0; i++)
        {
            Vector2Int pos = availablePositions[0];
            availablePositions.RemoveAt(0);
            grid[pos.y][pos.x].SetUp(CellType.Enemy, enemyDamage: 1, enemyIcon: "Trap"); // Если ловушки имеют отдельный тип
        }

        // Размещаем бонусы здоровья
        for (int i = 0; i < heartsCount && availablePositions.Count > 0; i++)
        {
            Vector2Int pos = availablePositions[0];
            availablePositions.RemoveAt(0);
            grid[pos.y][pos.x].SetUp(CellType.PowerUpHealth, powerUpHealth: 1);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void MoveUp()
    {
         MovePlayer(Vector2Int.up);
    }
    public void MoveLeft()
    {
        MovePlayer(Vector2Int.left);
    }
    public void MoveRight()
    {
        MovePlayer(Vector2Int.right);
    }
    public void MoveDown()
    {
        MovePlayer(Vector2Int.down);
    }
    private void MovePlayer(Vector2Int direction)
    {
        Vector2Int newPosition = playerPosition + direction;

        if (newPosition.x < 0 || newPosition.x >= gridWidth || newPosition.y < 0 || newPosition.y >= gridHeight)
        {
            Debug.Log("New position is out of bounds.");
            return;
        }

        previousPlayerPosition = playerPosition; // Сохраняем текущую позицию перед движением
        HandleCellInteraction(newPosition, direction);
    }

    private void HandleCellInteraction(Vector2Int position, Vector2Int direction)
    {
        Cell cell = grid[position.y][position.x];
        Debug.Log($"Handling interaction with cell of type: {cell.Type}");

        void Move()
        {
            grid[playerPosition.y][playerPosition.x].SetUp(CellType.Empty);
            playerPosition = position;
            grid[playerPosition.y][playerPosition.x].SetUp(CellType.Player);
        }

        if (cell.Type == CellType.Enemy)
        {
            Move();
            TakeDamage(cell.EnemyDamage);
            Debug.Log("Player hit by enemy!");
        }
        else if (cell.Type == CellType.PowerUpHealth)
        {
            currentHealth = Mathf.Min(3, currentHealth + cell.PowerUpHealth);
            UpdateHealth();
            Debug.Log("Health restored!");
            Move();
        }
        else if (cell.Type == CellType.Bunny)
        {
            CollectBunny();
            Move();
            if (collectedbunnies >= bunniesCount)
            {
                SoundManager.Instance.PlayClip(SoundManager.Instance.winSound);
                Win();
            }
            else
            {
                SoundManager.Instance.PlayClip(SoundManager.Instance.scoreSound);
            }
        }
        else
        {
            Move();
        }
    }

    private void CollectBunny()
    {      
        collectedbunnies++;
        Debug.Log($"Bunny colected. Total count: {collectedbunnies}");
        bunnyCountText.text = $"{collectedbunnies}/{bunniesCount}";
    }

    private void TakeDamage(int value)
    {
        currentHealth -= value;

        if(currentHealth <= 0)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.gameoverSound);
            currentHealth = 0;
            GameOver();
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.failSound);
        }

        UpdateHealth();
    }

    private void UpdateHealth()
    {
        foreach (Transform t in heartsContainer)
        {
            t.gameObject.SetActive(false);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            heartsContainer.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverFrame.sprite = loseFrame;
        tryAgainButton.SetActive(true);
        SoundManager.Instance.PlayClip(SoundManager.Instance.gameoverSound);
    }

    private void Win()
    {
        gameOverPanel.SetActive(true);
        gameOverFrame.sprite = winFrame;
        tryAgainButton.SetActive(false);

        SoundManager.Instance.PlayClip(SoundManager.Instance.winSound);

        int coins = Coins.GetCoins();

        coins += coinsForLevel;

        Coins.SaveCoins(coins);
    }
}
