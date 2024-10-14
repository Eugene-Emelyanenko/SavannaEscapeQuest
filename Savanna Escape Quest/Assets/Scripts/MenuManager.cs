using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TigerLevel
{
    public int level;
    public int meatPrice;
    public int waterPrice;
    public int coinsPrice;
    public int meatTime;
    public int waterTime;
    public int coinsTime;
    public int maxFood;

    public TigerLevel(int level, int meatPrice, int waterPrice, int coinsPrice, int meatTime, int waterTime, int coinsTime, int maxFood)
    {
        this.level = level;
        this.meatPrice = meatPrice;
        this.waterPrice = waterPrice;
        this.coinsPrice = coinsPrice;
        this.meatTime = meatTime;
        this.waterTime = waterTime;
        this.coinsTime = coinsTime;
        this.maxFood = maxFood;
    }
}

public static class TigerLevels
{
    public static List<TigerLevel> tigerLevels = new List<TigerLevel>
    {
        new TigerLevel(1, 50, 50, 300, 30, 30, 20, 120),
        new TigerLevel(2, 60, 60, 400, 25, 25, 15, 140),
        new TigerLevel(3, 50, 50, 300, 20, 20, 15, 160),
        new TigerLevel(4, 80, 80, 500, 15, 15, 10, 200),
        new TigerLevel(5, 100, 100, 600, 10, 10, 10, 240),
        new TigerLevel(6, 180, 180, 800, 5, 5, 5, 260)
    };

    public static TigerLevel GetLevel(int index)
    {
        return tigerLevels.Find(level => level.level == index);
    }
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject updatePanel;
    [SerializeField] private Image tigerImage;
    [SerializeField] private Button updateButton;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI meatText;
    [SerializeField] private Image waterFillImage;

    [SerializeField] private TextMeshProUGUI coinsPriceText;
    [SerializeField] private TextMeshProUGUI meatPriceText;
    [SerializeField] private TextMeshProUGUI waterPriceText;

    [SerializeField] private TextMeshProUGUI meatTimeText;
    [SerializeField] private TextMeshProUGUI waterTimeText;
    [SerializeField] private TextMeshProUGUI coinsTimeText;

    [SerializeField] private float superGamePartFoodPrice = 4f;
    [SerializeField] private int superGameCoinsPrice = 100;

    private const string lastUpdateKey = "LastUpdateTime";

    private TigerLevel currentLevel = null;

    private void Start()
    {
        currentLevel = TigerLevels.GetLevel(PlayerPrefs.GetInt("Tiger", 1));
        int nextLevel = currentLevel.level + 1;
        if (nextLevel >= TigerLevels.tigerLevels.Count)
            updateButton.interactable = false;

        updateButton.onClick.RemoveAllListeners();
        updateButton.onClick.AddListener(UpdateTigerLevel);

        EnableUpdatePanel(false);

        UpdateUI();

        // Вызываем метод для начисления ресурсов, которые должны были накопиться при выходе
        CheckOfflineProgress();

        // Запускаем корутины для регулярного начисления ресурсов
        StartCoroutine(CollectMeatRoutine());
        StartCoroutine(CollectWaterRoutine());
        StartCoroutine(CollectCoinsRoutine());
    }

    // Проверяем сколько времени прошло с момента последнего обновления и начисляем пропущенные ресурсы
    private void CheckOfflineProgress()
    {
        // Получаем текущее время и время последнего обновления
        DateTime currentTime = DateTime.Now;
        string lastUpdateString = PlayerPrefs.GetString(lastUpdateKey, currentTime.ToString());
        DateTime lastUpdateTime = DateTime.Parse(lastUpdateString);

        // Рассчитываем, сколько времени прошло
        TimeSpan timeDifference = currentTime - lastUpdateTime;

        // Получаем прошедшее время в минутах
        double totalMinutesPassed = timeDifference.TotalMinutes;

        // Рассчитываем, сколько ресурсов нужно начислить
        int meatToAdd = Mathf.FloorToInt((float)(totalMinutesPassed / currentLevel.meatTime));
        int waterToAdd = Mathf.FloorToInt((float)(totalMinutesPassed / currentLevel.waterTime));
        int coinsToAdd = Mathf.FloorToInt((float)(totalMinutesPassed / currentLevel.coinsTime));

        // Начисляем ресурсы
        AddMeat(meatToAdd);
        AddWater(waterToAdd);
        AddCoins(coinsToAdd);

        // Сохраняем текущее время как новое время последнего обновления
        PlayerPrefs.SetString(lastUpdateKey, currentTime.ToString());
        PlayerPrefs.Save();
    }

    // Сохраняем время выхода из игры/сцены
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(lastUpdateKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetString(lastUpdateKey, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }

    // Корутина для периодического получения мяса
    private IEnumerator CollectMeatRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentLevel.meatTime * 60); // Переводим минуты в секунды
            AddMeat(1); // Добавляем 1 мясо
        }
    }

    // Корутина для периодического получения воды
    private IEnumerator CollectWaterRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentLevel.waterTime * 60); // Переводим минуты в секунды
            AddWater(1); // Добавляем 1 воду
        }
    }

    // Корутина для периодического получения монет
    private IEnumerator CollectCoinsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentLevel.coinsTime * 60); // Переводим минуты в секунды
            AddCoins(1); // Добавляем 1 монету
        }
    }

    private void UpdateUI()
    {
        coinsText.text = Coins.GetCoins().ToString();

        meatText.text = Meat.GetMeat().ToString();

        int water = Water.GetWater();
        float fillAmount = (float)water / currentLevel.maxFood;
        waterFillImage.fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);

        coinsPriceText.text = currentLevel.coinsPrice.ToString();
        meatPriceText.text = currentLevel.meatPrice.ToString();
        waterPriceText.text = currentLevel.waterPrice.ToString();

        meatTimeText.text = $"- {currentLevel.meatTime.ToString()} min";
        waterTimeText.text = $"- {currentLevel.waterTime.ToString()} min";
        coinsTimeText.text = $"- {currentLevel.coinsTime.ToString()} min";

        tigerImage.sprite = Resources.Load<Sprite>($"Tiger/{currentLevel.level}");
    }

    public void PlaySuperGame()
    {
        int coins = Coins.GetCoins();
        int water = Water.GetWater();
        int meat = Meat.GetMeat();

        int waterPrice = Mathf.RoundToInt(currentLevel.waterPrice / superGamePartFoodPrice);
        int meatPrice = Mathf.RoundToInt(currentLevel.meatPrice / superGamePartFoodPrice);

        Debug.Log($"SuperGame price: {superGameCoinsPrice} coins, {waterPrice} water, {meatPrice} meat");

        if (coins >= superGameCoinsPrice && water >= waterPrice && meat >= meatPrice)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);

            coins -= superGameCoinsPrice;
            Coins.SaveCoins(coins);

            water -= waterPrice;
            Water.SaveWater(water);

            meat -= meatPrice;
            Meat.SaveMeat(meat);

            UpdateUI();

            SceneManager.LoadScene("SuperGame");
        }
        else
        {
            Debug.Log("Not enought");
        }
    }
    
    public void EnableUpdatePanel(bool isOpen)
    {
        updatePanel.SetActive(isOpen);

        UpdateUI();
    }

    private void UpdateTigerLevel()
    {
        int coins = Coins.GetCoins();
        int water = Water.GetWater();
        int meat = Meat.GetMeat();

        if (coins >= currentLevel.coinsPrice && water >= currentLevel.waterPrice && meat >= currentLevel.meatPrice)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);

            coins -= currentLevel.coinsPrice;
            Coins.SaveCoins(coins);

            water -= currentLevel.waterPrice;
            Water.SaveWater(water);

            meat -= currentLevel.meatPrice;
            Meat.SaveMeat(meat);

            int nextLevel = currentLevel.level + 1;
            if (nextLevel >= TigerLevels.tigerLevels.Count)
                updateButton.interactable = false;
            PlayerPrefs.SetInt("Tiger", nextLevel);
            PlayerPrefs.Save();
            currentLevel = TigerLevels.GetLevel(nextLevel);

            UpdateUI();
        }
        else
        {
            Debug.Log("Not enought");
        }
    }

    public void AddCoins(int value)
    {
        int coins = Coins.GetCoins();
        coins += value;
        Coins.SaveCoins(coins);
        UpdateUI();
    }

    public void AddWater(int value)
    {
        int water = Water.GetWater();
        water += value;
        Water.SaveWater(water);
        UpdateUI();
    }

    public void AddMeat(int value)
    {
        int meat = Meat.GetMeat();
        meat += value;
        Meat.SaveMeat(meat);
        UpdateUI();
    }
}
