using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using WebSocketSharp;

public static class Network : object {
	
	public class NetworkMsg{
		public string type; //connectRequest, position //connected, foundMatch, position
		public string msg;
		public string id;
	}
	
	public delegate void ServerConnectDel();
	public static ServerConnectDel ServerConnect;
	public delegate void ServerDisonnectDel();
	public static ServerDisonnectDel ServerDisonnect;
	public delegate void ServerErrorDel();
	public static ServerErrorDel ServerError;
	public delegate void ServerMsgDel(NetworkMsg msg);
	public static ServerMsgDel ServerMsg;
	
	
	static WebSocket ws;
	
	
	//network
	public static void ConnectToServer(){
		Game.GameState = Game.GameStateType.ConnectedToServer;
		
		ws = new WebSocket("ws://127.0.0.1:8080", "echo-protocol");		//production: ws://ec2-54-227-104-51.compute-1.amazonaws.com:8080/
		
		ws.OnOpen += (sender, e) =>
        {			
			Debug.Log("connection with server opened");
			if (ServerConnect != null) ServerConnect();
			Loom.QueueOnMainThread(()=>{ 
				ws.Send( JsonConvert.SerializeObject( new NetworkMsg(){ type = "connectRequest" }) );
			});
        };
			
		ws.OnMessage += delegate(object sender, MessageEventArgs e) {
			NetworkMsg ms = JsonConvert.DeserializeObject<NetworkMsg>(e.Data);
			//Debug.Log("from: " + ms.id + " type: " +  ms.type + " tata: " + ms.msg);
			if (ServerMsg != null) ServerMsg(ms);						
		};
        
        ws.OnError += delegate(object sender, ErrorEventArgs e) {
			Debug.Log("error: " + e.Message);
			if (ServerError!=null) ServerError();
        };

        ws.OnClose += delegate(object sender, CloseEventArgs e) {				        	
			Debug.Log("connection closed: " + e.Data);
			if (ServerDisonnect!=null) ServerDisonnect();			
        };
		
		ws.OnOpen+= delegate(object sender, System.EventArgs e) {			
		};
		
        #if DEBUG
        ws.Log.Level = LogLevel.TRACE;
        #endif
        
        ws.Connect();
	}
	
	public static void Send(NetworkMsg msg){
		ws.Send(JsonConvert.SerializeObject(msg));
	}
	
}
