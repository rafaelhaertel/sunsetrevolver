using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	public GameObject pauseMenu;
	public static bool finished = false;
	public static bool gameover = false;
	public static bool restart = false;
	public static bool pause = false;
	public Animator animator;

	public void Update() {
		if (finished)
			animator.SetTrigger ("FadeOut");

		if (EnergyBarScript.lives > 0 && restart)
			animator.SetTrigger ("Restart");


		if (EnergyBarScript.lives <= 0) {
			gameover = true;
			EnergyBarScript.lives = 2;
			EnergyBarScript.objectivepoints = 300000;
			EnergyBarScript.points = 0;
		}

		if (gameover)
			animator.SetTrigger ("GameOver");

		if (CrossPlatformInputManager.GetButtonDown ("Submit")) {
			Pause ();
		}

	}

	public void CarregarFase1(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex +1);
		finished = false;
	}

	public void CarregarRestart(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		restart = false;
	}

	public void Pause(){

		if(Time.timeScale == 1)
		{
			Time.timeScale = 0;
			pauseMenu.SetActive (true);

		} else if (Time.timeScale == 0){
			Time.timeScale = 1;
			pauseMenu.SetActive (false);
		}
		
	}

	public void GameOverLoader(){
		SceneManager.LoadScene ("MainMenu");
		finished = false;
		gameover = false;
	}
}
