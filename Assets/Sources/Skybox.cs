using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    public Material[] skyboxMaterial;

    void Start()
    {
        int sb = PlayerPrefs.GetInt("SKYBOX", -1);
        if (sb >= 0)
        {
            RenderSettings.skybox = skyboxMaterial[sb];
        }
    }

    public void ChangeSkybox(int i)
    {
        PlayerPrefs.SetInt("SKYBOX", i);
        RenderSettings.skybox = skyboxMaterial[i];
    }

    public void ChangeTileMaterial(int i)
    {
        PlayerPrefs.SetInt("MATERIAL", i);
    }
}
