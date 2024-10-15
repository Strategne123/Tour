using Mirror;
using UnityEngine;

public class CameraForServer : NetworkBehaviour
{
    private GameObject _mainCamera;
    private Camera _cameraComponent;

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
    }
}