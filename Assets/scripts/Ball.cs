using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	public GameObject AttachedTo;
	public float BallSpeed = 80;
	public GameObject LastTouched;
	
	public static float MaxSpeed = 30;
	float maxForce = 300;
	// Use this for initializationעשצק
	void Start () {
		Game.AddBall(this.gameObject);		
	}
	
	// Update is called once per frame
	void Update () {
		if (AttachedTo) 
		{			
			transform.position = AttachedTo.transform.position + new Vector3(0, (AttachedTo.GetComponent<Tab>() != null) ? .8f : -.8f,0);			
			if (Input.GetKeyDown( KeyCode.Space) ){
				AttachedTo = null;				
				AddForce(BallSpeed * Input.GetAxis( "Horizontal" ), BallSpeed);
			}
		}
	}
	
	public void AddForce(float x, float y){		
		
		if (x>maxForce) x = maxForce;
		else if (x<-maxForce) x = -maxForce;
		if (y>maxForce) y = maxForce;
		else if (y<-maxForce) y = -maxForce;
		
		rigidbody.AddForce(x, y, 0);
		
		
	}
}
