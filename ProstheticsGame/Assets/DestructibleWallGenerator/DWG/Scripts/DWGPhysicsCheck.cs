using UnityEngine;
using System.Collections;

public class DWGPhysicsCheck : MonoBehaviour {

	private float timer;
	private bool sleeping;

	void FixedUpdate(){
		if(GetComponent<Rigidbody>().IsSleeping()){ // Check if the rigidbody attached to this obeject is in a Sleep state
			if(!sleeping){ // If sleeping bool is false continue
				timer += Time.deltaTime; // Start the timer using deltatime
				if(timer > 2){ // If the timer is over 2 seconds continue
					GetComponent<Rigidbody>().isKinematic = true; // Set the rigidbody to isKinematic, this sets the rigidbody to ignore physics
					sleeping = true; // Sleeping is now true
					timer = 0; // Reset the timer
				}
			}
		} else { // If the rigidbody is NOT asleep continue
			sleeping = false; // Sleeping is now false
			timer = 0; // Reset the timer
		}
	}
}
