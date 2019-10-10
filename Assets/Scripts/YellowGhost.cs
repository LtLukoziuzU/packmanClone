using UnityEngine;
using System.Collections;

public class YellowGhost : MonoBehaviour {

	private int PacmanLayerMask = 512;
	private int IntersectionLayerMask = 1024;
	private int GroundLayerMask = 256;
	private int RespawnLayerMask = 4098;
	private bool chase = false, scare = false;										 //is the mode "Chase" or "Scatter"; is it scare mode?
	private char direct = 'L', oldDirect = 'L', d1, d2, d3, d4;
	private Vector2 RealSTarget = new Vector2(-3.610352f, -2.539203f), STarget, CTarget, position2D;
	private Vector3 position;
	private bool changeDir = false, blockL, blockR, blockD, blockU, inters;         //does it need to change direction; is ... side blocked; does the ghost stand on intersection
	private RaycastHit2D hit;
	private float distL, distR, distU, distD;                                     //distances from ghost to target tile
	private GameObject player, mtimer, rstrt;
	private char mode;
	public bool noteaten = true, itMoved = false, justEaten = false, canExit = false;   //is it not eaten, did it move when randoming move, was it just eaten(needed for repainting/changing mode), can it Exit
	private int randdir;
	private int steps = 3;                                                       //used for moving out and into ghost house
	public bool modeChange = false;                                               // is the ghost reversed; 
	private Sprite scaredGhost, eatenGhost, yellowGhost;
	
	
	
	//Find sprites and gameObjects, start up movement
	void Start () {
		scaredGhost = Resources.Load("scaredghost", typeof(Sprite)) as Sprite;
		eatenGhost = Resources.Load("eatenghost", typeof(Sprite)) as Sprite;
		yellowGhost = Resources.Load("orangeghost", typeof(Sprite)) as Sprite;
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		InvokeRepeating("Movement", 0.5f, 0.22f);
		player = GameObject.Find("Pacman");
		mtimer = GameObject.Find("Timer");
		rstrt = GameObject.Find("RespawnPlate");
		mode = mtimer.GetComponent<Modetimer>().mode;
		STarget = RealSTarget;
	}
	
	
	void Update () {
	//do nothing if ghost can't exit or is exiting/entering ghost house
	if (canExit) {
	if (steps != 6) {
	} else if ((scare == false) && (noteaten)) {
	//NORMAL MODE-----------------------------------------------------------------
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.1f, PacmanLayerMask);            //catching pacman
		if (hit.collider != null) {
			rstrt.GetComponent<RestartScript>().rstrt = true;
		}
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.05f, IntersectionLayerMask);      //catching intersection
		if (hit.collider != null) {
			changeDir = true;
			inters = true;
		} else {inters = false;}
		mode = mtimer.GetComponent<Modetimer>().mode;
		if (mode == 's') {                                                         //is it scare mode?
			scare = true;
			CancelInvoke();
			InvokeRepeating("Movement", 0.05f, 0.3f);
			this.GetComponent<SpriteRenderer>().sprite = scaredGhost;
		}
		//did the mode change?
		if ((chase == false) && (mode == 'C')) {
			chase = true;
		} else if ((chase) && (mode == 'S')) {
			chase = false;
		}
		//is yellow ghost too close pacman?
		if ((Mathf.Abs(position2D.x - player.transform.position.x) + Mathf.Abs(position2D.y - player.transform.position.y)) <= 2.4f) {
			modeChange = false;
			chase = false;
		}
		
	} else {
	//SCARE MODE------------------------------------------------------------------------------------
	if (noteaten) {
		if (mtimer.GetComponent<Modetimer>().mode != 's') {                 //reverting to normal mode if no more scare
			scare = false;
			CancelInvoke();
			if (mtimer.GetComponent<Modetimer>().mode != 'C') {
				chase = true;
			} else {chase = false;}
			STarget = RealSTarget;
			this.GetComponent<SpriteRenderer>().sprite = yellowGhost;
			InvokeRepeating("Movement", 0.1f, 0.22f);
		}
	} else {
	//EATEN MODE-----------------------------------------------------------------------------------------
		if (justEaten) {                   //setting up eaten mode
			CancelInvoke();
			InvokeRepeating("Movement", 0.05f, 0.18f);
			justEaten = false;
			this.GetComponent<SpriteRenderer>().sprite = eatenGhost;
		}
		chase = false;
		STarget = new Vector2(-0.01035586f, 1.060798f);
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.05f, IntersectionLayerMask);       //catching intersections
		if (hit.collider != null) {
			changeDir = true;
			inters = true;
		} else {inters = false;}
		
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.05f, RespawnLayerMask);          //respawning
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Respawn") {
				noteaten = true;
				this.GetComponent<SpriteRenderer>().sprite = scaredGhost;
				CancelInvoke();
				InvokeRepeating("Movement", 0.05f, 0.3f);
				steps = 0;
				mtimer.GetComponent<Modetimer>().yellowexit = false;
			}
		} 
	}
	}
	} 
	}
	
	void Movement () {
	//ENTERING/EXITING GHOST HOUSE-----------------------------------------------------------
	if (steps != 6) {
		if (steps < 3) {
			position = this.transform.position;
			if (steps <= 1) { position.y -= 0.3f; }
			if (steps == 2) { position.x += 0.3f; }
			this.transform.position = position;
			steps++;
		} else if (canExit) {
			position = this.transform.position;
			if (steps == 3) { position.x -= 0.3f; }
			if (steps >= 4) { position.y += 0.3f; }
			this.transform.position = position;
			steps++;
			if (steps == 6) {
				RaycastChecks(position);
			}
		}
	//NORMAL OR EATEN MOVE-------------------------------------------------------------------
	} else if ((scare == false) || (noteaten == false)) {
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
	//SCARE MOVE--------------------------------------------------------------------------------
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
		//Is .... direction blocked?------------------------------------------------------------------------
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
		
		//Did the mode change? If yes, revert direction
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
					CTarget = new Vector2(player.transform.position.x, player.transform.position.y);       //Yellow's chase target tile is Pacman
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
		position.x = 0.2896749f;
		position.y = 0.4607993f;
		position.z = -1;
		this.transform.position = position;
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		STarget = RealSTarget;
		CancelInvoke();
		steps = 3;
		noteaten = true;
		scare = false;
		canExit = false;
		direct = 'L';
		oldDirect = 'L';
		CancelInvoke();
		InvokeRepeating("Movement", 3f, 0.22f);
		this.GetComponent<SpriteRenderer>().sprite = yellowGhost;
	}
	
}
