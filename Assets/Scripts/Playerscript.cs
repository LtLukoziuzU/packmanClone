using UnityEngine;
using System.Collections;

public class Playerscript : MonoBehaviour {

	private int GroundLayerMask = 256;
	private int GhostLayerMask = 2048;
	public int Score = 0;             //232 points
	private int GScore = 0;            //8 ghost eaters 
	public Vector3 position;
	private Vector2 position2D;
	private bool movD = false;			//where is it moving when invoke hits up
	private bool movL = false;
	private bool movU = false;
	private bool movR = false;
	private char whatD = 'f';			//what is in what direction
	private char whatL = 'f';
	private char whatU = 'f';
	private char whatR = 'f';
	private RaycastHit2D hit;
	private GameObject mtimer;
	private bool ghosteat = false;		//can it eat ghosts
	private char mode = 'a';
	public char direct = 'R';			//pink and green ghost catches this for targetting
	private Sprite PacmanR, PacmanL, PacmanD, PacmanU;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	//Finding sprites and gameObjects, invoking movement and doing initial Raycasting around
	void Start () {
		PacmanR = Resources.Load("PacmanR", typeof(Sprite)) as Sprite;
		PacmanL = Resources.Load("PacmanL", typeof(Sprite)) as Sprite;
		PacmanU = Resources.Load("PacmanU", typeof(Sprite)) as Sprite;
		PacmanD = Resources.Load("PacmanD", typeof(Sprite)) as Sprite;
		InvokeRepeating ("Movement", 0.5f, 0.2f);
		mtimer = GameObject.Find("Timer");
		position = this.transform.position;
		RaycastChecks(position);
	}
	

	void Update () {
	//Changing direction if it can(pacman ignores command if it leads into blocked tile)
		if ( (Input.GetKey("down")) && (whatD != 'g') && (whatD != 'W') ) {
			movD = true;
			movL = false;
			movU = false;
			movR = false;
		}
		if ( (Input.GetKey("left")) && (whatL != 'g') && (whatL != 'W') ) {
			movD = false;
			movL = true;
			movU = false;
			movR = false;
		}
		if ( (Input.GetKey("up")) && (whatU != 'g') && (whatU != 'W') ) {
			movD = false;
			movL = false;
			movU = true;
			movR = false;
		}
		if ( (Input.GetKey("right")) && (whatR != 'g') && (whatR != 'W') ) {
			movD = false;
			movL = false;
			movU = false;
			movR = true;
		}
		//Can it eat ghosts due to scare mode---------------------------------------------------------------------
		if (mtimer.GetComponent<Modetimer>().mode == 's') {
			ghosteat = true;
		} else {ghosteat = false;}
		//Eating ghosts--------------------------------------------------------------------------------------------
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.1f, GhostLayerMask);
		if ((hit.collider != null) && (ghosteat)) {
			if (hit.collider.gameObject.tag == "Red") {
				if (hit.collider.gameObject.GetComponent<RedGhost>().noteaten) {
					hit.collider.gameObject.GetComponent<RedGhost>().justEaten = true;
					hit.collider.gameObject.GetComponent<RedGhost>().noteaten = false;
				}
			}
		}
		if ((hit.collider != null) && (ghosteat)) {
			if (hit.collider.gameObject.tag == "Pink") {
				if (hit.collider.gameObject.GetComponent<PinkGhost>().noteaten) {
					hit.collider.gameObject.GetComponent<PinkGhost>().justEaten = true;
					hit.collider.gameObject.GetComponent<PinkGhost>().noteaten = false;
				}
			}
		}
		if ((hit.collider != null) && (ghosteat)) {
			if (hit.collider.gameObject.tag == "Green") {
				if (hit.collider.gameObject.GetComponent<GreenGhost>().noteaten) {
					hit.collider.gameObject.GetComponent<GreenGhost>().justEaten = true;
					hit.collider.gameObject.GetComponent<GreenGhost>().noteaten = false;
				}
			}
		}
		if ((hit.collider != null) && (ghosteat)) {
			if (hit.collider.gameObject.tag == "Yellow") {
				if (hit.collider.gameObject.GetComponent<YellowGhost>().noteaten) {
					hit.collider.gameObject.GetComponent<YellowGhost>().justEaten = true;
					hit.collider.gameObject.GetComponent<YellowGhost>().noteaten = false;
				}
			}
		}
		//Returning back to normal mode----------------------------------------------------
		if ((mode == 's') && (mtimer.GetComponent<Modetimer>().mode != 's')) {
			ghosteat = false;
			CancelInvoke();
			InvokeRepeating("Movement", 0.05f, 0.2f);
			mode = 'a';
		}		
	}
	
	
	void Movement () {
		position = this.transform.position;
		if ( (movD) && (whatD != 'g') && (whatD != 'W') ) {
			position.y -= 0.3f;
			direct = 'D';
			this.GetComponent<SpriteRenderer>().sprite = PacmanD;
		}
		if ( (movL) && (whatL != 'g') && (whatL != 'W') ) {
			position.x -= 0.3f;
			direct = 'L';
			this.GetComponent<SpriteRenderer>().sprite = PacmanL;
		}
		if ( (movU) && (whatU != 'g') && (whatU != 'W') ) {
			position.y += 0.3f;
			direct = 'U';
			this.GetComponent<SpriteRenderer>().sprite = PacmanU;
		}
		if ( (movR) && (whatR != 'g') && (whatR != 'W') ) {
			position.x += 0.3f;
			direct = 'R';
			this.GetComponent<SpriteRenderer>().sprite = PacmanR;
		}
		this.transform.position = position;
		RaycastChecks(position);
	}
	
	
	
	void RaycastChecks (Vector3 position) {
		position2D.x = position.x;
		position2D.y = position.y;
		//Did it eat something?-------------------------------------------------------
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Point") {
				Score++;
				Object.Destroy(hit.collider.gameObject);
				mtimer.GetComponent<Modetimer>().ResetDotTimer();
				if (mtimer.GetComponent<Modetimer>().globalCounter) {
					mtimer.GetComponent<Modetimer>().dots++;
				}
				mtimer.GetComponent<Modetimer>().globalDots++;
			}
			if (hit.collider.gameObject.tag == "GhostEater") {
				GScore++;
				Object.Destroy(hit.collider.gameObject);
				mtimer.GetComponent<Modetimer>().ScareMode();
				CancelInvoke();
				InvokeRepeating("Movement", 0.15f, 0.15f);
			}
		}
		//What is in .... direction?----------------------------------------------------
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.4f, GroundLayerMask);
		if (hit.collider != null) {
			switch (hit.collider.gameObject.tag) {
				case "Wall" : whatR = 'W'; break;
				case "Point" : whatR = 'P'; break;
				case "GhostEater" : whatR = 'G'; break;
				case "Gate" : whatR = 'g'; break;
			}
		} else { whatR = 'E'; }
		
		position2D.x = position.x-0.4f;
		hit = Physics2D.Raycast(position2D, Vector2.right, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			switch (hit.collider.gameObject.tag) {
				case "Wall" : whatL = 'W'; break;
				case "Point" : whatL = 'P'; break;
				case "GhostEater" : whatL = 'G'; break;
				case "Gate" : whatL = 'g'; break;
			}
		} else { whatL = 'E'; }
		
		position2D.x = position.x;
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.4f, GroundLayerMask);
		if (hit.collider != null) {
			switch (hit.collider.gameObject.tag) {
				case "Wall" : whatU = 'W'; break;
				case "Point" : whatU = 'P'; break;
				case "GhostEater" : whatU = 'G'; break;
				case "Gate" : whatU = 'g'; break;
			}
		} else { whatU = 'E'; }
		
		position2D.y = position.y-0.4f;
		hit = Physics2D.Raycast(position2D, Vector2.up, 0.1f, GroundLayerMask);
		if (hit.collider != null) {
			switch (hit.collider.gameObject.tag) {
				case "Wall" : whatD = 'W'; break;
				case "Point" : whatD = 'P'; break;
				case "GhostEater" : whatD = 'G'; break;
				case "Gate" : whatD = 'g'; break;
			}
		} else { whatD = 'E'; }

		
		
	}
	
	//Restarting after pacman's death
	public void Restart() {
		position.x = -0.01034293f;
		position.y = -0.7392023f;
		position.z = -1;
		this.transform.position = position;
		position2D.x = this.transform.position.x;
		position2D.y = this.transform.position.y;
		CancelInvoke();
		InvokeRepeating("Movement", 3, 0.2f); 
		ghosteat = false;
		movD = false;
		movL = false;
		movU = false;
		movR = false;
		RaycastChecks(position);
	}
	
}
