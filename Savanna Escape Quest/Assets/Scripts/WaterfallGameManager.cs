using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaterfallGameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Transform itemsSpawnPoint;
    [SerializeField] private float xMaxSpawn = 3f;
    [SerializeField] private float xMinSpawn = -3f;
    [SerializeField] private float spawnDelay = 1f;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float xMaxPos;
    [SerializeField] private float xMinPos;

    [SerializeField] private Transform heartsContainer;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI collectedWaterText;

    private int currentHealth = 3;
    private int collectedWater = 0;
    private int xInput = 0;
    private bool isGameOver = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>($"Tiger/{PlayerPrefs.GetInt("Tiger", 1)}");
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        UpdateHealth();
        StartCoroutine(SpawnItems());
    }

    public void Move(bool isRight)
    {
        xInput = isRight ? 1 : -1;
    }

    public void StopMove()
    {
        xInput = 0;
    }

    private void Update()
    {
        if(!isGameOver)
        {
            // Получаем текущее положение объекта
            Vector3 currentPosition = transform.position;

            // Вычисляем новую позицию на основе скорости и направления
            float newXPosition = currentPosition.x + xInput * moveSpeed * Time.deltaTime;

            // Ограничиваем движение в пределах xMinPos и xMaxPos
            newXPosition = Mathf.Clamp(newXPosition, xMinPos, xMaxPos);

            // Обновляем позицию объекта
            transform.position = new Vector3(newXPosition, currentPosition.y, currentPosition.z);
        }
    }

    private void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        StopMove();
        gameOverPanel.SetActive(true);
        collectedWaterText.text = $"{collectedWater} -";
        Debug.Log("GameOver");
    }

    public void TakeDamage()
    {
        currentHealth--;
        
        if(currentHealth <= 0)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.winSound);
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

    public void TakeWater()
    {
        collectedWater++;
        int water = Water.GetWater();
        water++;
        Water.SaveWater(water);
        SoundManager.Instance.PlayClip(SoundManager.Instance.scoreSound);
    }

    private IEnumerator SpawnItems()
    {
        while (!isGameOver)
        {
            // Выбираем случайную позицию X в пределах от xMinSpawn до xMaxSpawn
            float randomX = Random.Range(xMinSpawn, xMaxSpawn);
            Vector3 spawnPosition = new Vector3(randomX, itemsSpawnPoint.position.y, itemsSpawnPoint.position.z);

            // Выбираем случайный префаб предмета из массива itemPrefabs
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject itemPrefab = itemPrefabs[randomIndex];

            // Создаем новый предмет на позиции спавна
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            // Ждем задержку перед спавном следующего предмета
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
