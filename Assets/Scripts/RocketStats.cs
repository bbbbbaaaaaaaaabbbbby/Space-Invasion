using Mirror;
using UnityEngine;

public class RocketStats : NetworkBehaviour
{
    [SyncVar] public int damage = 50;
    [SyncVar] public GameObject owner;

    public int speed = 100;
}