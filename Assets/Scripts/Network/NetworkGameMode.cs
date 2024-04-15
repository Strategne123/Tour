using System;
using Mirror;
using UnityEngine;


public class NetworkGameMode : Singleton<NetworkGameMode>
{
    [HideInInspector] public int GameMode;

    public Action<int> OnGameModeChanged;

    private void Start()
    {
        NetworkClient.RegisterHandler<GamemodeMessage>(SetGameMode);
    }


    public void SetGameMode(GamemodeMessage msg)
    {
        GameMode = msg.gameMode;
        OnGameModeChanged?.Invoke(GameMode);
    }
}
