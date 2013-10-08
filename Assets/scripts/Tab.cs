using UnityEngine;
using System.Collections;

public class Tab : MonoBehaviour {
	public float Speed = 10f;
	public string Id;
	
	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
		//move
		if (gameObject.tag == "Player") transform.Translate(Speed * Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal"));			
		else if (gameObject.tag == "Computer") followBall();
    }	
	
	void OnCollisionEnter( Collision col ) {
		if (col.collider.tag == "Ball") hitBall(col);
		
	}
	
	void followBall(){
		//track the closest ball
		GameObject closestBall = null; float maxDistance = 0;
		if (Game.Balls != null){
			foreach(GameObject ball in Game.Balls){
				var distance = Vector3.Distance(transform.position, ball.transform.position);
				if (distance > maxDistance) { maxDistance = distance; closestBall = ball; }
			}
			transform.position =  Vector3.Lerp(transform.position, new Vector3(closestBall.transform.position.x, transform.position.y , transform.position.z), Speed * Time.deltaTime);
		}
	}
	
	void hitBall(Collision col){
		Ball b = col.collider.GetComponent<Ball>();				
		b.LastTouched = this.gameObject;
		
		if (Mathf.Abs(col.collider.rigidbody.velocity.x) < Ball.MaxSpeed && Mathf.Abs(col.collider.rigidbody.velocity.y) < Ball.MaxSpeed)
		foreach (ContactPoint contact in col.contacts) {
			if( contact.thisCollider == collider ) {
				// This is the paddle's contact point
				float english = contact.point.x - transform.position.x;
				
				b.AddForce(b.BallSpeed * english, b.BallSpeed);
			}
		}
	}
	
}
