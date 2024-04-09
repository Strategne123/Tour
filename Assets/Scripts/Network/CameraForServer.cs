using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using Vrs.Internal;


public class CameraForServer : NetworkBehaviour
{
    private GameObject _mainCamera;

    private Camera _cameraComponent;

    private List<CubeButtonGaze> _activeElements = new List<CubeButtonGaze>();

    private void Start()
    {
        _mainCamera = GameObject.Find("MainCamera");
        _cameraComponent = GetComponent<Camera>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _cameraComponent.enabled = true;
    }

    private void Update()
    {
        if (NetworkClient.active)
        {
            _cameraComponent.transform.rotation = _mainCamera.transform.rotation;
        }
        // if (NetworkServer.active)
        // {
        //     Vector3 centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        //
        //     Ray ray = _cameraComponent.ScreenPointToRay(centerOfScreen);
        //
        //     RaycastHit hitInfo;
        //     if (Physics.Raycast(ray, out hitInfo))
        //     {
        //         CubeButtonGaze gazeButton = hitInfo.collider.gameObject.GetComponent<CubeButtonGaze>();
        //         if (gazeButton!=null)
        //         {
        //             gazeButton.OnGazeEnter();
        //             _activeElements.Add(gazeButton);
        //         }
        //         else
        //         {
        //             _activeElements.ForEach((gaze => gaze.OnGazeExit()));
        //             _activeElements.Clear();
        //         }
        //     }
        //     else
        //     {
        //         _activeElements.ForEach((gaze => gaze.OnGazeExit()));
        //         _activeElements.Clear();
        //     }
        // }
    }
}