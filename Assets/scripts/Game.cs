using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Game : MonoBehaviour {		
	public static List<GameObject> Balls = new List<GameObject>();
	public GameObject Player1;
	public GameObject Player2;
	public string ClientId;
	public string EnemyId;
	public GameObject Ball;
	public enum GameStateType { Start, ConnectingToServer, ConnectedToServer, PlayingVsPlayer, PlayingVsComputer };
	public static GameStateType GameState = GameStateType.Start;	
		
	public delegate void ServerConnectDel();
	public ServerConnectDel ServerConnect;	
	public delegate void SetPositionDel(Vector3 pos);
	public SetPositionDel SetPosition;	
		
	private static Game instance;
	
	public static Game Instance{  get{ if (instance == null) instance = new Game(); return instance;} }
	
	void Awake(){
		instance = this;
	}	
		
	// Use this for initialization
	void Start () {
		Network.ServerMsg += delegate(Network.NetworkMsg ms) {
			switch (ms.type) {
				case "connected": 					
					if (ServerConnect != null) ServerConnect();
						ClientId = ms.id;
										
					GameState = GameStateType.ConnectedToServer;					
					if (ServerConnect != null) ServerConnect();
					Application.LoadLevel("lvlMultiplayers");
					
				break;
				case "foundMatch":
					Debug.Log("Found match against" +  ms.msg);
					EnemyId = ms.msg.Split('|')[0];
				
					Loom.QueueOnMainThread(()=>{ 
						GameState = GameStateType.PlayingVsPlayer;
										
						Player1.transform.position = new Vector3(0,-9.2f,0);
						Player2.transform.position = new Vector3(0,9.2f,0);						
					
						if(ms.msg.Split('|')[1] == "down"){ Player1.tag = "Player"; Player2.tag = "Enemy"; }
						else { Player2.tag = "Player"; Player1.tag = "Enemy"; }
					
						SetPosition += delegate(Vector3 pos) {			  
							enemy().transform.position = pos;
				        };
						
					});									
				break;
				case "position":					
					Loom.QueueOnMainThread(()=>{ 
						if (SetPosition != null)
							SetPosition(new Vector3(float.Parse(ms.msg.Split('|')[0]), float.Parse(ms.msg.Split('|')[1]), 0));
					});
				break;
				default: Debug.Log("unknown server response"); break;
			}
		};
		
		Network.ServerDisonnect += () => {};
	}
	
	void Update () {	
		if (Game.GameState == Game.GameStateType.PlayingVsPlayer)								
			Loom.QueueOnMainThread(()=>{ 				
				Network.Send( new Network.NetworkMsg(){ type = "position", id = ClientId, msg = player().transform.position.x.ToString() + "|" + player().transform.position.y } );							
			});		
		
	}
	
	public static void AddBall(GameObject ball){
		Balls.Add(ball);
	}
		
	GameObject player(){
		return (Player1.tag == "Player") ? Player1 : Player2;
	}
	
	GameObject enemy(){
		return (Player1.tag == "Enemy") ? Player1 : Player2;
	}
		
}
