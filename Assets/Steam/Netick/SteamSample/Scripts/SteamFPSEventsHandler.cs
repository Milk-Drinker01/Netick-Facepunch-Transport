﻿using UnityEngine;
using Netick;

namespace Netick.Examples.Steam
{
    public class SteamFPSEventsHandler : NetworkEventsListner
    {
        public Transform    SpawnPos;
        public GameObject   PlayerPrefab;
        public bool         SpawnPlayerForHost = false;

        // This is called to read inputs.
        public override void OnInput(NetworkSandbox sandbox)
        {
            var input         = sandbox.GetInput<SteamFPSInput>();

            input.Movement    = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            input.ShootInput |= Input.GetMouseButtonDown(0);
            input.SprintInput = Input.GetKey(KeyCode.LeftShift);
        }

        // This is called on the server and the clients when the scene has been loaded.
        public override void OnSceneLoaded(NetworkSandbox sandbox)
        {
            if (sandbox.IsClient)
                return;

            for (int i = 0; i < sandbox.ConnectedPlayers.Count; i++)
            {
                // if SpawnPlayerForHost is set to false, we don't spawn a player for the server
                // index zero is the server player

                if (!SpawnPlayerForHost && i == 0)
                    continue;

                var p          = sandbox.ConnectedPlayers[i];

                var spawnPos   = SpawnPos.position + Vector3.left * (i);
                var player     = sandbox.NetworkInstantiate(PlayerPrefab, spawnPos, Quaternion.identity, p).GetComponent<SteamFPSController>();
                p.PlayerObject = player.gameObject;
            }
        }

        // This is called on the server when a client has connected.
        public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
        {
            var spawnPos        = SpawnPos.position + Vector3.left * (1 + sandbox.ConnectedPlayers.Count);
            var player          = sandbox.NetworkInstantiate(PlayerPrefab, spawnPos, Quaternion.identity, client).GetComponent<SteamFPSController>();
            client.PlayerObject = player.gameObject;
        }
    }
}