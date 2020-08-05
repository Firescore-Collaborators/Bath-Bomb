using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class movingDown : MonoBehaviour
{
    public ParticlesAreaManipulator lightPink_pam, darkPink_pam;
    public VelocityManipulator lightPink_vm, darkPink_vm;
    public float speed;
    public ParticleSystem bubble_particle;

    int time_int;
    private void Start()
    {
        InvokeRepeating("Time_f", 0,1);
        Invoke("Stop_func", 11);
        bubble_particle.gameObject.transform.DOMoveZ(0, 0);//.SetEase(Ease.Linear);
        bubble_particle.gameObject.transform.DOScale(new Vector3(1.7f, 1.7f, 1.7f), 1).OnComplete(OnComplete_func);
        
    }

    void OnComplete_func()
    {
        bubble_particle.gameObject.transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 15);
    }
    void Time_f()
    {
        time_int++;
        switch(time_int)
        {
            case 3:
                bubble_particle.startSize = 0.1f;
                bubble_particle.emissionRate = 400;
                break;

            case 5:
                bubble_particle.startSize = 0.07f;
                bubble_particle.emissionRate = 300;
                darkPink_pam.m_strength = Random.Range(200f, 500f);
                break;

            case 6:
                darkPink_pam.m_strength = 200f;
                break;

            case 7:
                lightPink_pam.m_strength = Random.Range(200f, 250f);
                bubble_particle.startSize = 0.05f;
                bubble_particle.emissionRate = 200;
                break;

            case 8:
                lightPink_pam.m_strength = 200f;
                break;

            case 10:
                bubble_particle.startSize = 0.02f;
                bubble_particle.emissionRate = 100;
                break;

            case 12:
                bubble_particle.gameObject.SetActive(false);
                break;



        }
        print(time_int);
    }


    void Update()
    {
        gameObject.transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void Stop_func()
    {
        
        lightPink_pam.m_strength = 0;
        darkPink_pam.m_strength = 0;
        lightPink_pam.m_radius = 0;
        darkPink_pam.m_radius = 0;
        lightPink_vm.m_fluidVelocitySpeed = 0;
        darkPink_vm.m_fluidVelocitySpeed = 0;
    }
}
