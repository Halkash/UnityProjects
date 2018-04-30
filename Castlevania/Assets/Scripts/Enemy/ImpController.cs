using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum States { Stay, Walk, Pursuit, Attack, Dodged}

public class ImpController : MonoBehaviour {
    private States currentState;
    List

	// Use this for initialization
	void Start ()
    {
        currentState = States.Stay;
	}
	
	// Update is called once per frame
	void Update ()

    {
		
	}
}
