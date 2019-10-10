using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {

	private GameObject pacman;
	public string scoring;
	private int score;


	void Start() {
		pacman = GameObject.Find("Pacman");
		score = pacman.GetComponent<Playerscript>().Score;
		Object.Destroy(pacman);
		scoring = "You ate " + score + " dots";
	}
	

	
	
	void OnGUI() {
		GUI.TextField(new Rect(Screen.width / 2 - 60, (1 * Screen.height / 3) - 30, 120, 60), scoring);
		if (GUI.Button(new Rect(Screen.width / 3 - 60, (2 * Screen.height / 3) - 30, 120, 60), "Retry!")) {
			Application.LoadLevel("scene");
		}
		if (GUI.Button(new Rect(2 * Screen.width / 3 - 60, (2 * Screen.height / 3) - 30, 120, 60), "Exit")) {
			Application.Quit();
		}
	}
	
}
