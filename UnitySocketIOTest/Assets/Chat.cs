using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Collections.Generic;

public class Chat : MonoBehaviour {
	WebSocket ws=null;

	// Use this for initialization
	void Awake () {
		Connect();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnDestroy(){
		if(ws!=null) ws.Close();
	}

	/// <summary>
	/// The message which store current input text.
	/// </summary>
	string message = "";
	/// <summary>
	/// The list of chat message.
	/// </summary>
	List<string> messages = new List<string>();
	
	/// <summary>
	/// Raises the GU event.
	///
	/// </summary>
	void OnGUI(){
		
		// Input text
		message = GUI.TextArea(new Rect(0,10,Screen.width * 0.7f,Screen.height / 10),message);
		
		// Send message button
		if(GUI.Button(new Rect(Screen.width * 0.75f,10,Screen.width * 0.2f,Screen.height / 10),"Send")){
			SendChatMessage();
		}
		
		// Show chat messages
		var l = string.Join("\n",messages.ToArray());
		var height = Screen.height * 0.1f * messages.Count;
		GUI.Label(
			new Rect(0,Screen.height * 0.1f + 10,Screen.width * 0.8f,height),
			l);
		
	}
	

	
	void Connect(){
		ws =  new WebSocket("ws://socket.nappers.jp:8888/");

		// called when websocket messages come.
		ws.OnMessage += (sender, e) =>
		{
			string s = e.Data;
			Debug.Log(string.Format( "Receive {0}",s));
			messages.Add("> " + e.Data);
			if(messages.Count > 10){
				messages.RemoveAt(0);
			}
		};

		ws.OnError += (sender, e) => 
		{
			Debug.Log("OnError");
		};
		ws.OnClose += (sender, e) => 
		{
			Debug.Log("OnClose");
			
		};
		ws.OnOpen += (sender, e) => 
		{
			Debug.Log("OnOpen");
			
		};

		ws.Connect();
		Debug.Log("Connect to " + ws.Url);
	}
	
	void SendChatMessage(){
		Debug.Log("Send message " + message);
		ws.Send(message);
		this.message = "";
	}
}