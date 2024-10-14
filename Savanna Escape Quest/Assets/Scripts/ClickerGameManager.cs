using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickerGameManager : MonoBehaviour
{
    [SerializeField] private Transform bunnyTransform;
    [SerializeField] private float bunnySpeed;
    [SerializeField] private float bunnyDistance;
    [SerializeField] private Transform tigerTransform;
    [SerializeField] private float tigerSpeed;
    [SerializeField] private float speedIncreaser;
    [SerializeField] private float tigerDistance;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private int meatForWin = 5;
    [SerializeField] private Button tapButton;
    [SerializeField] private ScrollBG scrollBG;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winFrame;
    [SerializeField] private GameObject loseFrame;

    private bool isGameOver = false;
    private bool isGameStarted = false;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        winFrame.SetActive(false);
        loseFrame.SetActive(false);

        scrollBG.StopGame();

        tapButton.onClick.RemoveAllListeners();
        tapButton.onClick.AddListener(() =>
        {
            isGameStarted = true;
            scrollBG.StartGame();
            tapButton.onClick.RemoveAllListeners();
            tapButton.onClick.AddListener(Tap);
        });
    }

    private void Tap()
    {
        if (!isGameOver)
        {
            tigerSpeed += speedIncreaser;
        }
    }

    private void Update()
    {
        if (isGameStarted && !isGameOver)
        {
            bunnyTransform.Translate(Vector3.up * bunnySpeed * Time.deltaTime);

            tigerTransform.Translate(Vector3.up * tigerSpeed * Time.deltaTime);

            float distanceToFinish = Vector2.Distance(bunnyTransform.position, finishTransform.position);

            float distanceToBunny = Vector2.Distance(tigerTransform.position, bunnyTransform.position);

            if (distanceToFinish <= bunnyDistance)
            {
                GameOver(false);
            }

            if (distanceToBunny <= tigerDistance)
            {
                GameOver(true);
            }
        }
    }

    private void GameOver(bool isWin)
    {
        isGameOver = true;
        scrollBG.StopGame();
        gameOverPanel.SetActive(true);

        if (isWin)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.winSound);
            int meat = Meat.GetMeat();
            meat += meatForWin;
            Meat.SaveMeat(meat);
            winFrame.SetActive(true);
            Debug.Log("Тигр победил!");
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.gameoverSound);
            loseFrame.SetActive(true);
            Debug.Log("Заяц добрался до финиша. Проигрыш.");
        }

        tapButton.onClick.RemoveAllListeners();
    }
}
