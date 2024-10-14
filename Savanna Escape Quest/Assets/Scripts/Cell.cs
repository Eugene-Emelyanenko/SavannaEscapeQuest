using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellType
{
    Empty,
    Enemy,
    PowerUpHealth,
    Player,
    Bunny,
}
public class Cell : MonoBehaviour
{
    [SerializeField] private Image cellIcon;
    public CellType Type { get; private set; }
    public int EnemyDamage;

    public string EnemyIcon { get; private set; }

    public int PowerUpHealth { get; private set; }

    public void SetUp(CellType type, string enemyIcon = "", int enemyDamage = 0, int powerUpHealth = 0)
    {
        Type = type;

        EnemyIcon = enemyIcon;

        if (type == CellType.Player)
        {
            cellIcon.sprite = Resources.Load<Sprite>($"Tiger/{PlayerPrefs.GetInt("Tiger", 1)}");
        }
        else if (type == CellType.Enemy)
        {
            EnemyDamage = enemyDamage;
            cellIcon.sprite = Resources.Load<Sprite>($"SuperGame/{enemyIcon}");
        }
        else if(type == CellType.PowerUpHealth)
        {
            PowerUpHealth = powerUpHealth;
            cellIcon.sprite = Resources.Load<Sprite>($"SuperGame/Heart");
        }
        else if (type == CellType.Bunny)
        {
            cellIcon.sprite = Resources.Load<Sprite>($"SuperGame/Bunny");
        }
        else
        {
            cellIcon.sprite = Resources.Load<Sprite>($"SuperGame/Transparent");
        }
    }
}
