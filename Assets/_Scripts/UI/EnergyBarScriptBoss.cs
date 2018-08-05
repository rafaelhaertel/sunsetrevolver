using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarScriptBoss : MonoBehaviour {

	Image Health;
	float maxenergy = 100f;
	public static float energy;
	// Use this for initialization
	void Start () {
		Health = GetComponent<Image> ();
		energy = maxenergy;
	}
	
	// Update is called once per frame
	void Update () {
		Health.fillAmount = energy / maxenergy;
	}
}
