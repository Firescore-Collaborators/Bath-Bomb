using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using DG.Tweening;

public class emitInvisibleParticles_script : MonoBehaviour
{
    public static emitInvisibleParticles_script ins;

    public ParticleSystem invisibleParticle_gm;
    public Color redColor, greenColor, blueColor, yellowColor;
    public Color currColorSelected_color;
    public Material currColor_mat;
    public ObiEmitter obiEmitter_obj;
    public ObiParticleRenderer obiParticleRenderer_obj;
    public GameObject spoon_gm;
    //public float RotateSpeed_float;
    //bool spoonRotate_bool;

    void Awake()
    {
        ins = this;

        currColor_mat.color = currColorSelected_color;
        InvokeRepeating("StopParticle_func", 0.5f, 1);
    }

    public void StartMixing_func()
    {
        //spoon_gm.SetActive(true);
        //spoonRotate_bool = true;
    }

    private void FixedUpdate()
    {
        //if(spoonRotate_bool)
        //    spoon_gm.transform.Rotate(0,transform.forward.y * -RotateSpeed_float * Time.deltaTime,0,Space.Self);
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "cubeParticle" && !other.GetComponent<stopDetection_script>().stopDetection_bool)
        {
            other.GetComponent<stopDetection_script>().stopDetection_bool = true;
            other.gameObject.GetComponent<MeshRenderer>().material = currColor_mat;
        }
     }

    public void SelectColor_func(int colorSelected)
    {
        switch (colorSelected)
        {
            case 1:
                currColorSelected_color = redColor;
                obiParticleRenderer_obj.particleColor = redColor;
                break;

            case 2:
                currColorSelected_color = greenColor;
                obiParticleRenderer_obj.particleColor = greenColor;
                break;

            case 3:
                currColorSelected_color = blueColor;
                obiParticleRenderer_obj.particleColor = blueColor;
                break;

            case 4:
                currColorSelected_color = yellowColor;
                obiParticleRenderer_obj.particleColor = yellowColor;
                break;
        }

        currColor_mat.color = currColorSelected_color;
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

    void StopParticle_func()
    {
        if (obiEmitter_obj.isEmitting)
        {
            invisibleParticle_gm.Play();
        }
        else
            invisibleParticle_gm.Stop();
    }

}
