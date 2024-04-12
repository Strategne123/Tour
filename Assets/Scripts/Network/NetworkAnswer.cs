using System;
using Mirror;
using UnityEngine;
using Vrs.Internal;


public class NetworkAnswer : NetworkBehaviour
{
    private SpriteButtonGaze _spriteButtonGaze;

    private void Awake()
    {
        _spriteButtonGaze = GetComponentInChildren<SpriteButtonGaze>();
    }

    private void OnEnable()
    {
        _spriteButtonGaze.OnGazeEntered += OnGazeEnter;
        _spriteButtonGaze.OnGazeExited += OnGazeExit;
    }

    [Command]
    private void OnGazeEnter()
    {
        _spriteButtonGaze.OnGazeEnter();
    }

    [Command]
    private void OnGazeExit()
    {
        _spriteButtonGaze.OnGazeExit();
    }
}
