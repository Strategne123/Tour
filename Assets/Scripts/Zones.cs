using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using Vrs.Internal;


public class Zones : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private List<GameObject> fields = new List<GameObject>();
    [SerializeField] private List<Quaternion> looks = new List<Quaternion>();
    [SerializeField] private Material material;
    [SerializeField] private Transform sphere;
    private float ipd;
    private VrsViewer vrsViewer;


    private static Zones self;

    private void Start()
    {
        if(self == null)
        {
            self = this;
        }
        Set(0);
        vrsViewer = GameObject.FindObjectOfType<VrsViewer>();
        InitIPD();
    }

    public static void Set(int num)
    {
        self.material.mainTexture = self.sprites[num].texture;
        self.sphere.rotation = self.looks[num];
        self.ActivateField(num);
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

    private void InitIPD()
    {
        if (PlayerPrefs.HasKey("IPD"))
        {
            ipd = PlayerPrefs.GetFloat("IPD");
        }
        else
        {
            ipd = 58;
        }
        ChangeIPD(0);
    }

    public void ChangeIPD(int value)
    {
        ipd = PlayerPrefs.GetFloat("IPD");
        if(ipd ==0)
        {
            ipd = 58;
        }
        if ((ipd + value > 70) || (ipd + value <= 50))
            return;
        ipd += value;
        PlayerPrefs.SetFloat("IPD",ipd);
        var dist = ipd / 1000.0f;
        vrsViewer.SetIpd(dist);
    }

    public void Quit()
    {
        Application.Quit();
    }


    


}
