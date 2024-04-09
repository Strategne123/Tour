using Mirror;
using UnityEngine;


public class NetworkSphere : NetworkBehaviour
{
    [SerializeField] private GameObject _viarusCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _viarusCamera.SetActive(true);
    }
}
