using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour {
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		switch (Game.GameState) {
		case Game.GameStateType.Start:						
			if (GUI.Button (new Rect(10,10,150,100), "Connect To Server")) {				
				Network.ConnectToServer();
			}
			break;
		case Game.GameStateType.ConnectingToServer: 
			GUI.Button (new Rect(10,10,150,100), "Connecting");
		break;	
		case Game.GameStateType.ConnectedToServer: 
			GUI.Button (new Rect(10,10,150,100), "Player id: " + Game.Instance.ClientId);
		break;
		default:
		break;
		}
		
		
		
	}
	
}
