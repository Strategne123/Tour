using System.Collections.Generic;
using UnityEngine;

public class Zones : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private List<GameObject> fields = new List<GameObject>();
    [SerializeField] private List<Quaternion> looks = new List<Quaternion>();
    [SerializeField] private Material material;
    [SerializeField] private Transform sphere;

    private void Start()
    {
        Set(0);
    }

    public void Set(int num)
    {
        material.mainTexture = sprites[num].texture;
        sphere.rotation = looks[num];
        ActivateField(num);
    }

    private void ActivateField(int num)
    {
        for(var i=0;i<fields.Count;i++)
        {
            if(i==num)
            {
                fields[i].gameObject.SetActive(true);
            }
            else
            {
                fields[i].gameObject.SetActive(false);
                var shows = fields[i].GetComponentsInChildren<Show>();
                foreach(var show in shows)
                {
                    show.Close();
                }
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }


    


}
