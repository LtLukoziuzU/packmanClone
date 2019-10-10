using UnityEngine;
using System.Collections;

public class RestartScript : MonoBehaviour {

	private GameObject player, red, pink, green, yellow, timer, GUI;
	private int lives = 3;
	public bool rstrt = false;
	public int Score;
	
	//finding gameObjects
	void Start () {
		timer = GameObject.Find("Timer");
		yellow = GameObject.Find("Yellow");
		green = GameObject.Find("Green");
		pink = GameObject.Find("Pink");
		red = GameObject.Find("Red");
		player = GameObject.Find("Pacman");
	}
	
	
	void Update () {
		//if all dots are collected, finish game
		if (player.GetComponent<Playerscript>().Score == 232) {
			Application.LoadLevel("RetryScene");
		}
		//if pacman is eaten, remove life, check if game is over, restart if not
		if (rstrt) {
			rstrt = false;
			lives--;
			if (lives == 0) { 
				Application.LoadLevel("RetryScene");
			} else {
				player.GetComponent<Playerscript>().Restart();
				red.GetComponent<RedGhost>().Restart();
				pink.GetComponent<PinkGhost>().Restart();
				green.GetComponent<GreenGhost>().Restart();
				yellow.GetComponent<YellowGhost>().Restart();
				timer.GetComponent<Modetimer>().Restart();
			}
		}
	}
}
