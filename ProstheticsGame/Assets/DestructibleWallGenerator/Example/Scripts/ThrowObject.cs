using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour {

	void Update(){
		if(Input.GetKeyDown(KeyCode.LeftControl)){
			Throw();
		}
	}
	
	// Create a sphere and throw it
	void Throw(){
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
		go.transform.position = transform.position + transform.forward;
		go.AddComponent<Rigidbody>(); 
		go.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f); 
		go.AddComponent<DWGDestroyer>();
		Destroy(go,5f);
	}
}
