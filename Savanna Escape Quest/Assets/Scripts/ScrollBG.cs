using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG : MonoBehaviour
{
    public float speed = 4f;
    public float xMin = -960f;

    private Vector3 startPos;
    private RectTransform rectTransform;
    private bool isGameStarted = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        startPos = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (isGameStarted)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (rectTransform.anchoredPosition.y < xMin)
                rectTransform.anchoredPosition = startPos;
        }       
    }

    public void StartGame()
    {
        isGameStarted = true;
    }

    public void StopGame()
    {
        isGameStarted = false;
    }
}
