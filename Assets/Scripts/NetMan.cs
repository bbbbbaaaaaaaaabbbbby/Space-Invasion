using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMan : NetworkManager
{
    bool playerSpawned;
    bool playerConnected;
    public struct PosMessage : NetworkMessage
    {
        public Vector3 vector3;
    }


    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message)
    {
        GameObject go = Instantiate(playerPrefab, message.vector3, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, go);
    }

    // public override void OnStartServer()
    // {
    //     base.OnStartServer();
    //     NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter);
    // }

    public void ActivatePlayerSpawn()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10f;
        pos = Camera.main.ScreenToWorldPoint(pos);

        PosMessage m = new PosMessage()
        {
            vector3 = pos
        };
        NetworkClient.Send(m);
        playerSpawned = true;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        playerConnected = true;
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Mouse0) && !playerSpawned && playerConnected)
    //     {
    //         ActivatePlayerSpawn();
    //     }
    // }
}