using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class ButtonManager : MonoBehaviour {

	public Animator animator;

	public void Update() {
		if (CrossPlatformInputManager.GetButtonDown ("Submit")) {
			animator.SetTrigger ("FadeOut");
		}
	}

	public void setAnimationTrigger() {
		animator.SetTrigger ("FadeOut");
	}

	public void CarregarFase1(){
		SceneManager.LoadScene ("FirstStage");
	}

	public void CarregarFase(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex +1);
	}

	public void GameOverLoader(){
		SceneManager.LoadScene ("MainMenu");
		LevelLoader.finished = false;
		LevelLoader.gameover = false;
	}
}