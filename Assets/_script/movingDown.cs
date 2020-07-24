using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingDown : MonoBehaviour
{
    public ParticlesAreaManipulator lightPink_pam, darkPink_pam;
    public VelocityManipulator lightPink_vm, darkPink_vm;
    public float speed;

    int time_int;
    private void Start()
    {
        InvokeRepeating("Time_f", 0,1);
        Invoke("Stop_func", 11);
    }
    void Time_f()
    {
        time_int++;
        switch(time_int)
        {
            case 5:
                darkPink_pam.m_strength = Random.Range(200f, 500f);
                break;

            case 6:
                darkPink_pam.m_strength = 200f;
                break;

            case 7:
                lightPink_pam.m_strength = Random.Range(200f, 250f);
                break;

            case 8:
                lightPink_pam.m_strength = 200f;
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
