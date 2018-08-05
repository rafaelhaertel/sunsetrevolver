using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject knight;

	private Vector3 offset;
	private Vector3 test;

	// Use this for initialization
	void Start () {
		offset = transform.position - knight.transform.position;
	}

	// Update is called once per frame
	void LateUpdate () {
		test.x = knight.transform.position.x + offset.x;
		test.z = knight.transform.position.z + offset.z;
		test.y = transform.position.y;
		transform.position = test;
	}
}
