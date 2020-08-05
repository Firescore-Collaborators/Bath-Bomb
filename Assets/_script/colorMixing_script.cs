using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Obi;

public class colorMixing_script : MonoBehaviour
{
    public float lerpSpeed_float;

    public Color redColor, greenColor, blueColor, yellowColor;
    public Color startColor_color, endColor_color;
    float startTime;
    public List<Renderer> allCubes_list = new List<Renderer>();

    public GameObject spoon_gm;
    bool startMixing_bool;
    float time;

    public ObiEmitter obiEmitter_obj;
    public ObiParticleRenderer obiParticleRenderer_obj;

    void Start()
    {
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            allCubes_list.Add(gameObject.transform.GetChild(i).gameObject.GetComponent<Renderer>());
        }
    }

    public void StartEmitter_func()
    {
        obiEmitter_obj.minPoolSize = 0.9f;
        Invoke("StopEmitter_func", 0.2f);
    }
    void StopEmitter_func()
    {
        obiEmitter_obj.minPoolSize = 1;
    }


    void Update()
    {
        if (startMixing_bool)
        {
            time = (Time.time - startTime) * lerpSpeed_float;
            for (int i = 0; i <= allCubes_list.Count - 1; i++)
            {
                allCubes_list[i].material.color = Color.Lerp(startColor_color, endColor_color, time);
            }
        }
    }

    public void StartMixing_func()
    {
        startTime = Time.time;
        time = 0;
        spoon_gm.SetActive(true);
        startMixing_bool = true;
        spoon_gm.transform.DOScale(new Vector3(0.9f, 2, 0.9f), 14);
    }

    public void SelectColor_func(int colorSelected)
    {
        switch(colorSelected)
        {
            case 1:
                obiParticleRenderer_obj.particleColor = redColor;
                endColor_color = redColor;
                break;

            case 2:
                obiParticleRenderer_obj.particleColor = greenColor;
                endColor_color = greenColor;
                break;

            case 3:
                obiParticleRenderer_obj.particleColor = blueColor;
                endColor_color = blueColor;
                break;

            case 4:
                obiParticleRenderer_obj.particleColor = yellowColor;
                endColor_color = yellowColor;
                break;
        }
    }
}
