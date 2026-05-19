using System;
using UnityEngine;
using TMPro;

public class UserStats : MonoBehaviour
{
    public int hp = 100;
    public int points;
    public int speed = 16;
    public float fire_rate = 0.2f;
    public int dmg = 50;
    public int maneuverability;
    public TMP_Text points_text;
    public KeyCode shootButton =  KeyCode.Space;

    private void Start()
    {
        points_text = GameObject.Find("Points").GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        points_text.text = points.ToString();
    }
}
