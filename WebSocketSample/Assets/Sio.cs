using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SocketIOClient;


public class Sio : MonoBehaviour {

	private Client socket;
	string userName = "Crux";
	string message = "";
	string socketId;

	List<string> messages = new List<string>();


	// Use this for initialization
	void Start () {
		string url = "http://54.199.161.166:3000/";
		this.socket = new Client(url);

		this.socket.Opened += this.SocketOpened;
		this.socket.Message += this.SocketMessage;
		this.socket.SocketConnectionClosed += this.SocketConnectionClosed;
		this.socket.Error += this.SocketError;
		this.socket.Connect();


		// register for 'online' event with io server
		this.socket.On ("online", (data) => {
			Debug.Log ("online event: ");
			Debug.Log (data.Json.args[0]);
		});

		this.socket.On ("socketId", (data) => {
			Debug.Log ("socket id: ");
			Debug.Log (data.Json.args[0]);
		});

		this.socket.On ("sayToAllResponse", (data) => {
			Debug.Log ("to all: ");
			Debug.Log (data.Json.ToJsonString());
		});


		
	}

	


	// Update is called once per frame
	void Update () {


	}


	void OnGUI(){
		
		// Input text
		message = GUI.TextArea(new Rect(0, 10, Screen.width * 0.7f, Screen.height / 10), message);

		// Send message button
		if (GUI.Button(new Rect(Screen.width * 0.75f, 10, Screen.width * 0.2f, Screen.height / 10), "Send")){
			SendChatMessage();
			// SocketConnectionClosed();

		}



		// Show chat messages
		var l = string.Join("\n", messages.ToArray());
		var height = Screen.height * 0.1f * messages.Count;
		GUI.Label(
			new Rect(0, Screen.height * 0.2f + 10, Screen.width * 0.8f, height), l);
		
	}

 	
	void SendChatMessage(){
		Debug.Log("Send message: " + message);

		this.socket.Emit("sayToAll", message);
		
		this.message = "";
	}
	
	private void SocketOpened (object sender, EventArgs e){
		// Debug.Log("socket opened");
		this.socket.Emit("newUser", userName);
	}
	
	private void SocketMessage (object sender, MessageEventArgs e) {
		if ( e!= null && e.Message.Event == "message") {
			string msg = e.Message.MessageText;
			Debug.Log("socket message: " + msg);
		}
	}
	
	private void SocketConnectionClosed (object sender, EventArgs e) {
		Debug.Log("socket closed");
		this.socket.Emit("disconnect", message);
	}
	
	private void SocketError (object sender, EventArgs e) {
		Debug.Log("socket error: ");
	}
}
