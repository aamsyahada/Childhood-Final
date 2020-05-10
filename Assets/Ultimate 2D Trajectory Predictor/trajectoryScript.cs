using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class trajectoryScript : MonoBehaviour 

{

	public Sprite dotSprite;					
	public bool changeSpriteAfterStart;			
	public float initialDotSize;				
	public int numberOfDots;					
	public float dotSeparation;					
	public float dotShift;						
	public float idleTime;						
	private GameObject trajectoryDots;			
	private GameObject ball;					
	private Rigidbody2D ballRB;					
	private Vector3 ballPos;					
	private Vector3 fingerPos;					
	private Vector3 ballFingerDiff;				
	private Vector2 shotForce;					
	private float x1, y1;						
	private GameObject helpGesture;				
	private float idleTimer = 7f;				
	private bool ballIsClicked = false;			
	private bool ballIsClicked2 = false;		
	private GameObject ballClick;				
	public float shootingPowerX;				
	public float shootingPowerY;				
	public bool usingHelpGesture;				
	public bool explodeEnabled;					
	public bool grabWhileMoving;				
	public GameObject[] dots;					
	public bool mask;
	private BoxCollider2D[] dotColliders;
	public AudioSource hitEffect;

	void Start () {
		ball = gameObject;											
		ballClick = GameObject.Find ("Ball Click Area");			
		trajectoryDots = GameObject.Find ("Trajectory Dots");		
		if (usingHelpGesture == true) {								
			helpGesture = GameObject.Find ("Help Gesture");			
		}
		ballRB = GetComponent<Rigidbody2D> ();						

		trajectoryDots.transform.localScale = new Vector3 (initialDotSize, initialDotSize, trajectoryDots.transform.localScale.z); 

		for (int k = 0; k < 40; k++) {
			dots [k] = GameObject.Find ("Dot (" + k + ")");			
			if (dotSprite != null) {								
				dots [k].GetComponent<SpriteRenderer> ().sprite = dotSprite;	
			}
		}
		for (int k = numberOfDots; k < 40; k++) {					
			GameObject.Find ("Dot (" + k + ")").SetActive (false);	
		}
		trajectoryDots.SetActive (false);							
	

		}
	

		

	void Update () {

		if (numberOfDots > 40) {
			numberOfDots = 40;
		}

		if (usingHelpGesture == true) {								
			helpGesture.transform.position = new Vector3 (ballPos.x, ballPos.y, ballPos.z);	
		}

		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);	

		if (hit.collider != null && ballIsClicked2 == false) {					
			if (hit.collider.gameObject.name == ballClick.gameObject.name) {	
				ballIsClicked = true;											
			} else {															
				ballIsClicked = false;											
			}
		} else {																
			ballIsClicked = false;												
		}

		if (ballIsClicked2 == true) {											
			ballIsClicked = true;												
		}


		if ((ballRB.velocity.x * ballRB.velocity.x) + (ballRB.velocity.y * ballRB.velocity.y) <= 0.0085f) { 
			ballRB.velocity = new Vector2 (0f, 0f);								
			idleTimer -= Time.deltaTime;										
			
		} else {																
			trajectoryDots.SetActive (false);									
		}

		if (usingHelpGesture == true && idleTimer <= 0f) {						
			helpGesture.GetComponent<Animator> ().SetBool ("Inactive", true);	
		}
	

		ballPos = ball.transform.position;										

		if (changeSpriteAfterStart == true) {									
			for (int k = 0; k < numberOfDots; k++) {
				if (dotSprite != null) {										
					dots [k].GetComponent<SpriteRenderer> ().sprite = dotSprite;
				}
			}
		}


		if ((Input.GetKey (KeyCode.Mouse0) && ballIsClicked == true) && ((ballRB.velocity.x == 0f && ballRB.velocity.y == 0f) || (grabWhileMoving == true))) {	
			ballIsClicked2 = true;												

			if (usingHelpGesture == true) {										
				idleTimer = idleTime;											
				helpGesture.GetComponent<Animator> ().SetBool ("Inactive", false);	
			}

			fingerPos = Camera.main.ScreenToWorldPoint (Input.mousePosition); 	
			fingerPos.z = 0;													

			if (grabWhileMoving == true) {										
				ballRB.velocity = new Vector2 (0f, 0f);							
				ballRB.isKinematic = true;										
		}

			ballFingerDiff = ballPos - fingerPos;								
			
			shotForce = new Vector2 (ballFingerDiff.x * shootingPowerX, ballFingerDiff.y * shootingPowerY);

			if ((Mathf.Sqrt ((ballFingerDiff.x * ballFingerDiff.x) + (ballFingerDiff.y * ballFingerDiff.y)) > (0.4f))) { 
				trajectoryDots.SetActive (true);								
			} else {
				trajectoryDots.SetActive (false);								
				if (ballRB.isKinematic == true) {
					ballRB.isKinematic = false;
				}
			}

			for (int k = 0; k < numberOfDots; k++) {							
				x1 = ballPos.x + shotForce.x * Time.fixedDeltaTime * (dotSeparation * k + dotShift);	
			y1 = ballPos.y + shotForce.y * Time.fixedDeltaTime * (dotSeparation * k + dotShift) - (-Physics2D.gravity.y/2f * Time.fixedDeltaTime * Time.fixedDeltaTime * (dotSeparation * k + dotShift) * (dotSeparation * k + dotShift));	
				dots [k].transform.position = new Vector3 (x1, y1, dots [k].transform.position.z);	
			}
		}


		if (Input.GetKeyUp (KeyCode.Mouse0)) {								
		
			ballIsClicked2 = false;											

			if (trajectoryDots.activeInHierarchy) {							
				if(explodeEnabled == true){									
				StartCoroutine(explode ());									
			}
			trajectoryDots.SetActive (false);								
				ballRB.velocity = new Vector2 (shotForce.x, shotForce.y);	
				if (ballRB.isKinematic == true) {							
					ballRB.isKinematic = false;								
			}
		}
	}
}

	public IEnumerator explode(){										
		yield return new WaitForSeconds (Time.fixedDeltaTime * (dotSeparation * (numberOfDots - 1f)));	
		Debug.Log ("exploded");
	

	

	}

	public void collided(GameObject dot){

		for (int k = 0; k < numberOfDots; k++) {
			if (dot.name == "Dot (" + k + ")") {
				
				for (int i = k + 1; i < numberOfDots; i++) {
					
					dots [i].gameObject.GetComponent<SpriteRenderer> ().enabled = false;
				}

			}

		}
	}
	public void uncollided(GameObject dot){
		for (int k = 0; k < numberOfDots; k++) {
			if (dot.name == "Dot (" + k + ")") {

				for (int i = k-1; i > 0; i--) {
				
					if (dots [i].gameObject.GetComponent<SpriteRenderer> ().enabled == false) {
						Debug.Log ("");
						return;
					}
				}

				if (dots [k].gameObject.GetComponent<SpriteRenderer> ().enabled == false) {
					for (int i = k; i > 0; i--) {
						
						dots [i].gameObject.GetComponent<SpriteRenderer> ().enabled = true;

					}

				}
			}

		}
	}
	void OnCollisionEnter2D(Collision2D other) {
    	if(other.collider.name=="Tanah"){
    		StartCoroutine(jeda());
		}if(other.collider.tag=="Player"){
            hitEffect.Play();
		}

	IEnumerator jeda(){
    	ballRB.velocity = Vector2.zero;
    	ballRB.GetComponent<Transform>().position = new Vector2(8,-3);

    	yield return new WaitForSeconds(2);    
    }
	}
}

