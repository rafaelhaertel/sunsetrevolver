using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarScript : MonoBehaviour {

	public Text MaxPoints;
	public Text MaxPointsBack;
	public Text Lives;
	public Text LivesBack;
	public static float objectivepoints = 300000;
	Image Health;
	float maxenergy = 100f;
	public static float energy;
	public static int points;
	public static int lives = 2 ;
	// Use this for initialization
	void Start () {
		Health = GetComponent<Image> ();
		energy = maxenergy;
	}
	
	// Update is called once per frame
	void Update () {
		Health.fillAmount = energy / maxenergy;
		MaxPoints.text = points.ToString();
		MaxPointsBack.text = MaxPoints.text;
		Lives.text = lives.ToString ();
		LivesBack.text = Lives.text;
		if (points >= objectivepoints) {
			lives++;
			objectivepoints = objectivepoints + 300000;
		}
			
	}
}
