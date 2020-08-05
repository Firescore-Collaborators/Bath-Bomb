using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popSoap_script : MonoBehaviour
{
    public float yForce_float, rotForce_float;
    Rigidbody tempRb;
    
    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "pop")
        {
            coll.gameObject.GetComponent<BoxCollider>().enabled = false;
            tempRb = coll.gameObject.AddComponent<Rigidbody>();
            tempRb.AddForce(0, Random.Range(1,5), coll.gameObject.transform.position.z, ForceMode.Impulse);
            //tempRb.AddTorque(transform.forward * rotForce_float * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
