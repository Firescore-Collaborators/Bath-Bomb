using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Obi;

//[RequireComponent(typeof(ObiSolver))]
public class ColliderHighlighter : MonoBehaviour {

 	public ObiSolver solver;

	void Awake(){
		//solver = GetComponent<Obi.ObiSolver>();
        
        InvokeRepeating("CallFunc", 0, 2);
        //InvokeRepeating("StopFunc", 1, 2);
    }

    void CallFunc()
    {
        solver.OnCollision += Solver_OnCollision;
    }
    void StopFunc()
    {
        //solver.OnCollision -= Solver_OnCollision;
    }


    void OnEnable () {
		//solver.OnCollision += Solver_OnCollision;
	}

	void OnDisable(){
		solver.OnCollision -= Solver_OnCollision;
	}
	
	/*void Solver_OnCollision (object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
	{
        print("s");
		Oni.Contact[] contacts = e.contacts.Data;
		for(int i = 0; i < e.contacts.Count; ++i)
		{
			Oni.Contact c = contacts[i];
                // make sure this is an actual contact:
                //if (c.distance < 0.01f)
                //{
                // get the collider:
                Collider collider = ObiCollider.idToCollider[c.other] as Collider;
                if(collider.gameObject.name.Contains("Cube") && !collider.GetComponent<stopDetection_script>().stopDetection_bool)
                {
                    print("11");
                    e.contacts.Remove(contacts[i]);
                    collider.GetComponent<stopDetection_script>().stopDetection_bool = true;
                    break;
                    
                }
				//if (collider != null){
				//	// make it blink:
				//	Blinker blinker = collider.GetComponent<Blinker>();
	            //
				//	if (blinker)
				//		blinker.Blink();
				//}
			//}
		}


	}
    */
    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
    {
        Oni.Contact contact = e.contacts[0];
        //print("11");
        Component collider;
        if (ObiCollider.idToCollider.TryGetValue(contact.other, out collider))
        {
            print("22");
            if (collider.gameObject.name.Contains("Cube") && !collider.GetComponent<stopDetection_script>().stopDetection_bool)
            {
                print("33");
                e.contacts.Remove(e.contacts[0]);
                collider.GetComponent<stopDetection_script>().stopDetection_bool = true;
            }

            
        }
    }




}
