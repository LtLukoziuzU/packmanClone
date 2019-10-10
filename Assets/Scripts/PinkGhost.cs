using UnityEngine;
using System.Collections;

public class PinkGhost : MonoBehaviour {

	private int PacmanLayerMask = 512;
	private int IntersectionLayerMask = 1024;
	private int GroundLayerMask = 256;
	private int RespawnLayerMask = 4098;
	private bool chase = false, modeChange = false, scare = false;						//is the mode "Chase" or "Scatter"; is the ghost reversed; is it scare mode?
	private char direct = 'L', oldDirect = 'L';
	private Vector2 RealSTarget = new Vector2(-3.610326f, 2.560799f), STarget, CTarget, position2D;
	private Vector3 position;
	private bool changeDir = false, blockL, blockR, blockD, blockU, inters;			//does it need to change direction; is ... side blocked; does the ghost stand on intersection
	private RaycastHit2D hit;
	private float distL, distR, distU, distD;										//distances from ghost to target tile
	private GameObject player, mtimer, rstrt;
	private char mode;
	public bool noteaten = true, itMoved = false, justEaten = false, canExit = true;	//is it not eaten, did it move when randoming move, was it just eaten(needed for repainting/changing mode), can it Exit
	private int randdir;
	private int steps = 2;										//used when moving in and out of ghost house
	private Sprite scaredGhost, eatenGhost, pinkGhost;
	
	
	//Finding sprites and gameObjects, starting movement
	void Start () {
		scaredGhost = Resources.Load("scaredghost", typeof(Sprite)) as Sprite;
		eatenGhost = Resources.Load("eatenghost", typeof(Sprite)) as Sprite;
		pinkGhost = Resources.Load("pinkghost", typeof(Sprite)) as Sprite;
		InvokeRepeating("Movement", 0.5f, 0.22f);
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		player = GameObject.Find("Pacman");
		mtimer = GameObject.Find("Timer");
		rstrt = GameObject.Find("RespawnPlate");
		mode = mtimer.GetComponent<Modetimer>().mode;
		STarget = RealSTarget;
	}
	
	
	void Update () {
	//do nothing if ghost can't exit or is moving in or out of ghost house
	if (canExit) {
	if (steps != 4) {												
	} else if ((scare == false) && (noteaten)) {
	//NORMAL MODE--------------------------------------------------------------------------------------
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.1f, PacmanLayerMask);               //catching pacman
		if (hit.collider != null) {
			rstrt.GetComponent<RestartScript>().rstrt = true;
		}
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.05f, IntersectionLayerMask);    //catching intersections
		if (hit.collider != null) {
			changeDir = true;
			inters = true;
		} else {inters = false;}
		mode = mtimer.GetComponent<Modetimer>().mode;
		if (mode == 's') {                                                      //is it scare mode
			scare = true;
			CancelInvoke();
			InvokeRepeating("Movement", 0.05f, 0.3f);
			this.GetComponent<SpriteRenderer>().sprite = scaredGhost;
		}
		if ((chase == false) && (mode == 'C')) {                                //did the mode change?
			modeChange = true; 
			chase = true;
		} else if ((chase) && (mode == 'S')) {
			modeChange = true;
			chase = false;
		}
	} else {
	//SCARE MODE------------------------------------------------------------------------------------------------------
	if (noteaten) {
		if (mtimer.GetComponent<Modetimer>().mode != 's') {                 //did the scare mode finish?
			scare = false;
			CancelInvoke();
			if (mtimer.GetComponent<Modetimer>().mode != 'C') {
				chase = true;
			} else {chase = false;}
			STarget = RealSTarget;
			this.GetComponent<SpriteRenderer>().sprite = pinkGhost;
			InvokeRepeating("Movement", 0.1f, 0.22f);
		}
	//EATEN MODE------------------------------------------------------------------------------------------------------
	} else {
		if (justEaten) {                                                           //setting up eaten mode
			CancelInvoke();
			InvokeRepeating("Movement", 0.05f, 0.18f);
			justEaten = false;
			this.GetComponent<SpriteRenderer>().sprite = eatenGhost;
		}
		chase = false;
		STarget = new Vector2(-0.01035586f, 1.060798f);
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.05f, IntersectionLayerMask);      //catching intersections
		if (hit.collider != null) {
			changeDir = true;
			inters = true;
		} else {inters = false;}
		
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.05f, RespawnLayerMask);          //respawning
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Respawn") {
				noteaten = true;
				this.GetComponent<SpriteRenderer>().sprite = scaredGhost;
				steps = 0;
				CancelInvoke();
				InvokeRepeating("Movement", 0.05f, 0.3f);
				mtimer.GetComponent<Modetimer>().pinkexit = false;
			}
		} 
	}
	}
	}
	}
	
	void Movement () {
	if (steps != 4) {
	//Entering or exiting ghost house---------------------------------------
		if (steps < 2) {
			position = this.transform.position;
			position.y -= 0.3f;
			this.transform.position = position;
			steps++;
		} else if (canExit) {
			position = this.transform.position;
			position.y += 0.3f;
			this.transform.position = position;
			steps++;
			if (steps == 4) {
				RaycastChecks(position);
			}
		}
	} else if ((scare == false) || (noteaten == false)) {
	//NORMAL OR EATEN MOVE----------------------------------------------------
		position = this.transform.position;
		RaycastChecks(position);
		if ( (direct == 'D') ) {
			position.y -= 0.3f;
		}
		if ( (direct == 'L') ) {
			position.x -= 0.3f;
		}
		if ( (direct == 'U') ) {
			position.y += 0.3f;
		}
		if ( (direct == 'R') ) {
			position.x += 0.3f;
		}
		this.transform.position = position;
	} else {
	//SCARE MOVE-----------------------------------------------------------
		position = this.transform.position;
		RaycastChecks(position);
		itMoved = false;
		while (itMoved == false) {
			randdir = Random.Range(1, 4);
			if ((blockU == false) && (randdir == 1)) { position.y += 0.3f; itMoved = true;}
			if ((blockR == false) && (randdir == 2)) { position.x += 0.3f; itMoved = true;}
			if ((blockD == false) && (randdir == 3)) { position.y -= 0.3f; itMoved = true;}
			if ((blockL == false) && (randdir == 4)) { position.x -= 0.3f; itMoved = true;}
		}
		this.transform.position = position;
	}
	}
	
	
	void RaycastChecks(Vector3 position) {
		position2D.x = position.x+0.2f;
		position2D.y = position.y;
		
		//Is .... direction blocked?-------------------------------------------------------------------
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			if ((hit.collider.gameObject.tag == "Wall") || (hit.collider.gameObject.tag == "Gate")) {
				blockR = true;
				if (direct == 'R') {
					changeDir = true;
				}
			} else {blockR = false;}
		} else {blockR = false;}
		
		position2D.x = position.x-0.4f;
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			if ((hit.collider.gameObject.tag == "Wall") || (hit.collider.gameObject.tag == "Gate")) {
				blockL = true;
				if (direct == 'L') {
					changeDir = true;
				}
			} else {blockL = false;}
		} else {blockL = false;}
		
		position2D.x = position.x;
		position2D.y = position.y+0.2f;
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			if ((hit.collider.gameObject.tag == "Wall") || (hit.collider.gameObject.tag == "Gate")) {
				blockU = true;
				if (direct == 'U') {
					changeDir = true;
				}
			} else {blockU = false;}
		} else {blockU = false;}
		
		position2D.y = position.y-0.4f;
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			if ((hit.collider.gameObject.tag == "Wall") || (hit.collider.gameObject.tag == "Gate")) {
				blockD = true;
				if (direct == 'D') {
					changeDir = true;
				}
			} else {blockD = false;}
		} else {blockD = false;}
		position2D.y = position.y;
		//Did the mode change? If yes, reverse.
		if (modeChange) {
				if (direct == 'L') { 
					direct = 'R'; oldDirect = 'R'; 
				} else if (direct == 'R') { 
					direct = 'L'; oldDirect = 'L'; 
				} else if (direct == 'D') { 
					direct = 'U'; oldDirect = 'U'; 
				} else { 
					direct = 'D'; oldDirect = 'D'; 
				}
		}
		//If mode didn't change but it still got request to change direction(due to finding obstacle or intersection)
		if ((changeDir) && (modeChange == false)) {
			if (inters == false) {
				if ((blockL == false) && (oldDirect != 'R')) { direct = 'L'; }
				if ((blockD == false) && (oldDirect != 'U')) { direct = 'D'; }
				if ((blockU == false) && (oldDirect != 'D')) { direct = 'U'; }
				if ((blockR == false) && (oldDirect != 'L')) { direct = 'R'; }
			} else {
				if (chase == false) {
					PickDirS();
				} else {
					CTarget = new Vector2(player.transform.position.x, player.transform.position.y);                   //Pink's chase target is 4 tiles towards Pacman's direction
					if (player.GetComponent<Playerscript>().direct == 'L') { CTarget.x -= 1.2f; }                      
					if (player.GetComponent<Playerscript>().direct == 'R') { CTarget.x += 1.2f; }
					if (player.GetComponent<Playerscript>().direct == 'D') { CTarget.y -= 1.2f; }
					if (player.GetComponent<Playerscript>().direct == 'U') { CTarget.x -= 1.2f; CTarget.y += 1.2f; }   //Left original overflow bug for up direction
					PickDirC();
				}
			}
			oldDirect = direct;
		}
		oldDirect = direct;
		modeChange = false;
		changeDir = false;
	}
	
	//Picking direction while scattered
	void PickDirS() {
		distL = Mathf.Abs((position2D.x - 0.3f) - STarget.x) + Mathf.Abs(position2D.y - STarget.y); 
		distR = Mathf.Abs((position2D.x + 0.3f) - STarget.x) + Mathf.Abs(position2D.y - STarget.y); 
		distD = Mathf.Abs(position2D.x - STarget.x) + Mathf.Abs((position2D.y - 0.3f) - STarget.y); 
		distU = Mathf.Abs(position2D.x - STarget.x) + Mathf.Abs((position2D.y + 0.3f) - STarget.y); 
		if ((blockL == true) || (direct == 'R')) { distL = 10000; }
		if ((blockR == true) || (direct == 'L')) { distR = 10000; }
		if ((blockD == true) || (direct == 'U')) { distD = 10000; }
		if ((blockU == true) || (direct == 'D')) { distU = 10000; }
		if ((distU <= distR) && (distU <= distL) && (distU <= distD)) { direct = 'U'; }
		if ((distD <= distR) && (distD <= distU) && (distD <= distL)) { direct = 'D'; }
		if ((distL <= distR) && (distL <= distU) && (distL <= distD)) { direct = 'L'; }
		if ((distR <= distL) && (distR <= distU) && (distR <= distD)) { direct = 'R'; }
	}
	
	//Picking direction while chasing
	void PickDirC() {
		distL = Mathf.Abs((position2D.x - 0.3f) - CTarget.x) + Mathf.Abs(position2D.y - CTarget.y); 
		distR = Mathf.Abs((position2D.x + 0.3f) - CTarget.x) + Mathf.Abs(position2D.y - CTarget.y); 
		distD = Mathf.Abs(position2D.x - CTarget.x) + Mathf.Abs((position2D.y - 0.3f) - CTarget.y); 
		distU = Mathf.Abs(position2D.x - CTarget.x) + Mathf.Abs((position2D.y + 0.3f) - CTarget.y); 
		if ((blockL == true) || (direct == 'R')) { distL = 10000; }
		if ((blockR == true) || (direct == 'L')) { distR = 10000; }
		if ((blockD == true) || (direct == 'U')) { distD = 10000; }
		if ((blockU == true) || (direct == 'D')) { distU = 10000; }
		if ((distU <= distR) && (distU <= distL) && (distU <= distD)) { direct = 'U'; }
		if ((distD <= distR) && (distD <= distU) && (distD <= distL)) { direct = 'D'; }
		if ((distL <= distR) && (distL <= distU) && (distL <= distD)) { direct = 'L'; }
		if ((distR <= distL) && (distR <= distU) && (distR <= distD)) { direct = 'R'; }
	}
	
	
	//Restarting after pacman's death
	public void Restart() {
		position.x = -0.01034293f;
		position.y = 0.4607983f;
		position.z = -1;
		this.transform.position = position;
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		STarget = RealSTarget;
		CancelInvoke();
		InvokeRepeating("Movement", 3, 0.22f); 
		noteaten = true;
		scare = false;
		steps = 2;
		this.GetComponent<SpriteRenderer>().sprite = pinkGhost;
		canExit = false;
		CancelInvoke();
		InvokeRepeating("Movement", 3f, 0.22f);
	}
	
}
