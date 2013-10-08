using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Game : MonoBehaviour {		
	public static List<GameObject> Balls;
	public GameObject Player1;
	public GameObject Player2;	
	public GameObject Ball;
	public enum GameStateType { Start, ConnectingToServer, ConnectedToServer, PlayingVsPlayer, PlayingVsComputer };
	public static GameStateType GameState = GameStateType.Start;	
		
	public delegate void ServerConnectDel();
	public ServerConnectDel ServerConnect;	
	public delegate void SetPositionDel(string type, Vector3 pos);
	public SetPositionDel SetPosition;	
		
	private static Game instance;
	
	static string positionInGame; //up/down
	public static string enemyId;
	public static string clientId;	
	
	public static Game Instance{  get{ if (instance == null) instance = new Game(); return instance;} }
	
	void Awake(){
		instance = this;
	}	
		
	// Use this for initialization
	void Start () {
		Debug.Log("level: " + Application.loadedLevel);
		
		Balls = new List<GameObject>();			
		
		Network.ServerMsg += delegate(Network.NetworkMsg ms) {
			switch (ms.type) {
				case "connected": 					
					if (ServerConnect != null) ServerConnect();
						clientId = ms.id;
								
					GameState = GameStateType.ConnectedToServer;										
					Loom.QueueOnMainThread(()=>{ 
						if (ServerConnect != null) ServerConnect();						
					});
					
				break;
				case "foundMatch":
					Debug.Log("Found match against" +  ms.msg);
					enemyId = ms.msg.Split('|')[0];
					positionInGame = ms.msg.Split('|')[1];
				
					Loom.QueueOnMainThread(()=>{ 
						Application.LoadLevel("lvlMultiplayers");																	
					});									
				break;
				case "position":					
					Loom.QueueOnMainThread(()=>{
						if (SetPosition != null) SetPosition(ms.msg.Split('|')[0], new Vector3(float.Parse(ms.msg.Split('|')[1]), float.Parse(ms.msg.Split('|')[2]), 0));
					});
				break;
				case "enemyLeft":
					Debug.Log("enemy left game");
					GameState = GameStateType.Start;
					Loom.QueueOnMainThread(()=>{ 
						Application.LoadLevel("lvl1");																	
					});		
				break;
				default: Debug.Log("unknown server response"); break;
			}
		};
		
		Network.ServerDisonnect += () => {};
		
		Network.ServerDisonnect += () => {
			Loom.QueueOnMainThread(()=>{ 
				Application.LoadLevel("lvl1");
			});
		};
	}
	
	void Update () {	
		if (Game.GameState == Game.GameStateType.PlayingVsPlayer)								
			Loom.QueueOnMainThread(()=>{ 				
				Network.Send( new Network.NetworkMsg(){ type = "position", id = clientId, msg = "t|" + player().transform.position.x.ToString() + "|" + player().transform.position.y } );
				if (Player1.tag == "Player") //only player1 update ball location
					Network.Send( new Network.NetworkMsg(){ type = "position", id = clientId, msg = "b|" + Ball.transform.position.x.ToString() + "|" + Ball.transform.position.y } );				
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
	
	void OnLevelWasLoaded(int level) {
		if (level == 1){
			GameState = GameStateType.PlayingVsPlayer;
															
			if(positionInGame == "down"){ Player1.tag = "Player"; Player2.tag = "Enemy"; }
			else { Player2.tag = "Player"; Player1.tag = "Enemy"; }
		
			SetPosition += delegate(string type, Vector3 pos) {			  
				if (type == "t") enemy().transform.position = pos;
				else if (type == "b") Ball.transform.position = pos;
	        };			
		}
	}
		
}
