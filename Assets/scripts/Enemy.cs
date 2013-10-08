using UnityEngine;
using System.Collections;

public class Enemy : Tab {
	
	// Use this for initialization
	void Start () {
		Speed = 4;
		
	}
	
	// Update is called once per frame
	void Update () {
		followBall();
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
}
