using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Vrs.Internal;


public class NetworkQuestion : NetworkBehaviour
{
    private List<SpriteButtonGaze> _spriteButtonGazes;

    private void Awake()
    {
        _spriteButtonGazes = GetComponentsInChildren<SpriteButtonGaze>().ToList<SpriteButtonGaze>();
    }

    private void OnEnable()
    {
        for (int i = 0; i < _spriteButtonGazes.Count; i++)
        {
            var index = i;
            _spriteButtonGazes[i].OnGazeEntered += (() => OnGazeEnter(index));
            _spriteButtonGazes[i].OnGazeExited += (() => OnGazeExit(index));
        }
        
    }

    private void OnDisable()
    {
        for (int i = 0; i < _spriteButtonGazes.Count; i++)
        {
            var index = i;
            _spriteButtonGazes[i].OnGazeEntered -= (() => OnGazeEnter(index));
            _spriteButtonGazes[i].OnGazeExited -= (() => OnGazeExit(index));
        }
    }


    [Command]
    private void OnGazeEnter(int index)
    {
        _spriteButtonGazes[index].OnGazeEnter();
    }

    [Command]
    private void OnGazeExit(int index)
    {
        _spriteButtonGazes[index].OnGazeExit();
    }
}
