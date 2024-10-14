using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ItemCategory
{
    Water,
    Stone
}

public class Item : MonoBehaviour
{
    [SerializeField] private ItemCategory itemCategory;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float yMinPos = -10f;

    private WaterfallGameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<WaterfallGameManager>();
    }

    private void Update()
    {
        Fall();
    }

    private void Fall()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < yMinPos)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (itemCategory == ItemCategory.Stone)
                gameManager.TakeDamage();
            else
                gameManager.TakeWater();

            Destroy(gameObject);
        }
    }
}
