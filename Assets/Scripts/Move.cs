using UnityEngine;

public class Move : MonoBehaviour, IInteract
{
    [SerializeField] private int toZone;
    [SerializeField] private Zones zone;

    public void Interact()
    {
        print("To Zone " + toZone);
        zone.Set(toZone-1);
    }
}
