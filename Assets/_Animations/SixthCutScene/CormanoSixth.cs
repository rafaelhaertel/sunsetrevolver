using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CormanoSixth : MonoBehaviour {
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Walk() {
		anim.SetTrigger ("Walk");
	}
}
