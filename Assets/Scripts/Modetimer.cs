using UnityEngine;
using System.Collections;

public class Modetimer : MonoBehaviour {

	private float timer, chaseTime = 20, scatterTime = 7, pauseTime, scareTime = 6, dott = 4, dottimer;
	private int wave;
	public char mode = 'S', pausedMode;
	private GameObject pink, green, yellow;	
	public bool pinkexit, greenexit = false, yellowexit = false, globalCounter = false;
	public int dots = 0, globalDots = 0;
	
	//Finding needed gameObjects, starting both mode and no dot eating timers
	void Start () {
		pink = GameObject.Find("Pink");
		green = GameObject.Find("Green");
		yellow = GameObject.Find("Yellow");
		timer = Time.time + scatterTime;
		dottimer = dott + Time.time;
		wave = 1;
		pinkexit = true;
	}
	

	void Update () {
		//Global counter and releasing ghosts logic ---------------------------------------------------------------
		if (globalCounter) {
			if ((dots == 7) && (pinkexit == false)) {
				pink.GetComponent<PinkGhost>().canExit = true;
				pinkexit = true;
			}
			if ((dots == 17) && (greenexit == false)) {
				green.GetComponent<GreenGhost>().canExit = true;
				greenexit = true;
			}
			if ((dots == 32) && (yellowexit == false)) {
				globalCounter = false;
			}
		} else {
			if (globalDots >= 30) {
				green.GetComponent<GreenGhost>().canExit = true;
				greenexit = true;
			}
			if (globalDots >= 60) {
				yellow.GetComponent<YellowGhost>().canExit = true;
				yellowexit = true;
			}
		}
		//No dot eating timer with ghost releasing------------------------------------------------
		if (dottimer <= Time.time) {
			dottimer = dott + Time.time;
			if (pinkexit == false) {
				pinkexit = true;
				pink.GetComponent<PinkGhost>().canExit = true;
			} else if (greenexit == false) {
				greenexit = true;
				green.GetComponent<GreenGhost>().canExit = true;
			} else if (yellowexit == false) {
				yellowexit = true;
				yellow.GetComponent<YellowGhost>().canExit = true;
			}
		}
		//Mode timer-----------------------------------------------------------------------------
		if ((timer <= Time.time) && (wave != 8) && (mode != 's')) {
			if ((wave == 1) || (wave == 3) || (wave == 5)) {
				timer = Time.time + chaseTime;
				wave++;
				mode = 'C';
				yellow.GetComponent<YellowGhost>().modeChange = true;
			} else if ((wave == 2) || (wave == 4) || (wave == 6)) {
				if (wave == 4) {
					scatterTime = 5;
				}
				timer = Time.time + scatterTime;
				wave++;
				mode = 'S';
				yellow.GetComponent<YellowGhost>().modeChange = true;
			} else if (wave == 7) {
				timer = Time.time + 10000;
				wave++;
				mode = 'C';
				yellow.GetComponent<YellowGhost>().modeChange = true;
			}
		}
		//Removing pause if it's scare mode
		if ((timer <= Time.time) && (mode == 's')) {
			mode = pausedMode;
			timer = Time.time + pauseTime;
		}
	}
	
	public char getMode() {
		return mode;
	}
	
	//starting up scare timer
	public void ScareMode() {
		if (mode != 's') {
			pausedMode = mode;
			pauseTime = timer - Time.time;
			mode = 's';
			timer = Time.time + scareTime;
		} else {
			timer = Time.time + scareTime;
		}
	}
	
	public void ResetDotTimer() {
		dottimer = dott + Time.time;
	}
	
	//Restarting after pacman died
	public void Restart() {
		scatterTime = 7;
		globalCounter = true;
		timer = Time.time + scatterTime + 3;
		dottimer = dott + Time.time + 3;
		wave = 1;
		mode = 'S';
		pinkexit = false;
		greenexit = false;
		yellowexit = false;
		dots = 0;
	}
	
}
