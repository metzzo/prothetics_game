using UnityEngine;
using System.Collections;

public class DWGColliderEnabler : MonoBehaviour {
	void Start () {
		if(gameObject.GetComponent<Collider>()){ // If this game object has a collider, continue
			gameObject.GetComponent<Collider>().enabled = true; // Enable the collider
			Destroy(GetComponent<DWGColliderEnabler>()); // Remove this script from the game object
		}
	}
}
