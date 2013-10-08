using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour {
	public int Life = 2;
	public int MaxLife = 2;
	
	GameObject LastTouched;
	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter( Collision col ) {		
		if (col.collider.tag == "Ball"){		
			
			var ballLastTouched = col.collider.gameObject.GetComponent<Ball>().LastTouched;
			
			if (LastTouched == null){				
				LastTouched = ballLastTouched;
				renderer.material.color = LastTouched.renderer.material.color;				
				Life --;
			} else { //has color
				if (ballLastTouched == LastTouched) Life--;
				else Life++;
			}
			
			if (Life <= 0) Destroy(gameObject);
			else if (Life >= MaxLife) {renderer.material.color = Color.black; LastTouched = null; }
			
		}
	}
}
