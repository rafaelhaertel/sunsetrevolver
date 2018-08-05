using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCollider : MonoBehaviour {

	Quaternion rotation;
	// Use this for initialization
//	void Awake() {
//		rotation = transform.rotation;
//	}

	void FixedUpdate () {
		transform.rotation = new Quaternion(transform.rotation.x, 0f, transform.rotation.z, transform.rotation.w);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		transform.rotation = new Quaternion(transform.rotation.x, 0f, transform.rotation.z, transform.rotation.w);
	}
}