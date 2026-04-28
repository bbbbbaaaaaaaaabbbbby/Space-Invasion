using System;
using UnityEngine;

public class BuyUpgrades : MonoBehaviour
{
    public int points;
    private const int dmg_upgrade = 20;
    private const int fire_rate_upgrade = 30;
    private const int speed_upgrade = 10;
    private GameObject player;


    private void FixedUpdate()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        points = player.GetComponent<UserStats>().points;
    }
    
    public void UpgradeDmg()
    {
        if (points >= dmg_upgrade)
        {
            player.GetComponent<UserStats>().points -= dmg_upgrade;
            player.GetComponent<UserStats>().dmg *= 2;
        }
    }
    
    public void UpgradeFireRate()
    {
        if (points >= fire_rate_upgrade)
        {
            player.GetComponent<UserStats>().points -= fire_rate_upgrade;
            player.GetComponent<UserStats>().fire_rate /= 2;
        }
    }
    
    public void UpgradeSpeed()
    {
        if (points >= speed_upgrade)
        {
            player.GetComponent<UserStats>().points -= speed_upgrade;
            player.GetComponent<UserStats>().speed *= 2;
        }
    }
}
