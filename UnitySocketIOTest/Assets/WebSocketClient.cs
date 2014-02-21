using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Collections.Generic;


public class WebSocketClient : MonoBehaviour {
	private WebSocket ws;
	private string userID = "guest";
	// Use this for initialization
	void Start () {
		Connect();
	}
	
	// Update is called once per frame
	void Update () {
		
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
			//string jsonData = "{ \"message\" :\"getform\"}";
			ws.Send(message);
		}
		
		// Show chat messages
		var l = string.Join("\n",messages.ToArray());
		var height = Screen.height * 0.1f * messages.Count;
		GUI.Label(
			new Rect(0,Screen.height * 0.1f + 10,Screen.width * 0.8f,height),
			l);
	}
	

	
	void Connect(){
		ws =  new WebSocket("ws://localhost:8888/");
		
		// called when websocket messages come.
		ws.OnMessage += (sender, e) =>
		{
			/*
			string s = e.Data;
			Debug.Log(string.Format( "Receive {0}",s));
			messages.Add("> " + e.Data);
			if(messages.Count > 10){
				messages.RemoveAt(0);
			}
			*/

			var jsonData = MiniJSON.Json.Deserialize(e.Data) as Dictionary<string,object>;
			string message = (string)jsonData["message"];
			if(message == "returnform")
			{
				//get Map Data
			}
			else if(message == "requestcreateform")
			{
				//createMap
			}
			else if(message == "returnscore")
			{
				// rannkinnguitiran
			}
			else if(message == "sendposition")
			{
				//anotherUserPosition
				Debug.Log("a");
			}
			else if(message == "setuserid")
			{
				SetUserID((string)jsonData["userID"]);
			}
			Debug.Log(message);
		};
		ws.OnError += (sender, e) => 
		{
			Debug.Log("aa");
		};
		ws.OnClose += (sender, e) => 
		{
			Debug.Log("Closing...");

		};
		ws.Connect();
		Debug.Log("Connect to " + ws.Url);
	}
	
	void SendChatMessage(){
		Debug.Log("Send message " + message);
		ws.Send(message);
		this.message = "";
	}

	// Send Score
	void AddScore(int score)
	{
		string jsonData = "{ \"message\":\"addscore\", \"userID\":" + userID + ", \"score\":" + score + "}";
		ws.Send(jsonData);
	}

	//Get Form
	void GetForm(int formNum)
	{
		string jsonData = "{ \"message\": \"getform\", \"formNum\": " + formNum + "}";
		ws.Send(jsonData);
	}

	//Form Send
	void SendForm(int formNum,int[,] cell)
	{
		string cellString = "[";
		//y ziku
		for(int i=0; i < cell.Length; ++i)
		{
			//x ziku
			cellString += "[";
			for(int j = 0; j < cell.GetLength(i) ; ++j)
			{
				cellString += cell[j,i];
				if(j + 1 != cell.GetLength(i))
				{
					cellString += ",";
				}
			}
			cellString += "]";
		}
		cellString += "]";
		string jsonData = "{ \"message\": \"sendform\", \"formNum:\" " + formNum + ", \"formData\":" + cellString + "}";
		ws.Send(jsonData);
	}

	//Send User Profile
	void CreateUser(string user,string pass)
	{
		string jsonData = "{ \"message\":\"createuser\", \"userID\" : " + user + ",  \"password\":" + pass + "}";
		ws.Send(jsonData);
	}

	//Send My Position
	void UpdatePosition(int x,int y)
	{
		string jsonData = "{ \"message\":\"updateposition\", \"userID\": " + userID + ",  \"userX\":" + x + ", \"userY\":" + y + "}";
		ws.Send(jsonData);
	}

	void SetUserID(string user)
	{
		userID = user;
		Debug.Log(userID);
	}

	//application Quit
	void OnApplicationQuit()
	{
		string jsonData = "{ \"message\":\"delete\", \"userID\":\"" + userID + "\"}";
		ws.Send(jsonData);
		ws.Close();
	}
}
