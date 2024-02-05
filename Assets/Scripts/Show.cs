using RenderHeads.Media.AVProVideo;
using UnityEngine;
public class Show : MonoBehaviour, IInteract
{
    [SerializeField] private GameObject objectToShow;
    [SerializeField] private MediaPlayer video;

    private void Start()
    {
        video = objectToShow.GetComponent<MediaPlayer>();
    }
    public void Interact()
    {
        objectToShow.SetActive(true);
        if (video != null)
        {
            video.Play();
        }
    }

    public void Close()
    {
        if (video != null)
        {
            video.Stop();
            video.Rewind(true);
        }
        objectToShow.SetActive(false);
    }
}