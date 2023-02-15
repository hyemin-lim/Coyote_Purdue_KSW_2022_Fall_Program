/* 
 * // WebSocket Communication //
 * Code to receive the location of the coyote detected in real time from the server.
 * Lat means latitude, Lng means longitude.
 */

using UnityEngine;
using System.Globalization;
using WebSocketSharp;
using Assets.SimpleAndroidNotifications;
using System;

public class Node : MonoBehaviour
{
    //Coyote Localization value
    private WebSocket ws;//Socket Declaration

    //Start event function invokes once between Awake and Update function call when component is active
    void Start()
    {
        ws = new WebSocket("ws://YourIP:PORT"); //Insert Your Server IP, PORT
        ws.OnOpen += ws_OnOpen;//Register a function to run if the server is connected
        ws.OnMessage += ws_OnMessage; //Register a function to run when a message comes from the server toward Unity.
        ws.OnClose += ws_OnClose;//Register a function to run when the server is closed.
        ws.Connect();//Connect to the server.
        ws.Send("Client : Connected"); //Send a message to the server
    }

    //real-time Get message
    void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data); //received data Debug

        //Trim received data with Split
        string data = e.Data;
        string[] codes = data.Split(',');

        //string to double convert
        NumberFormatInfo provider = new NumberFormatInfo();
        provider.NumberDecimalSeparator = ".";
        double latitude = System.Convert.ToDouble(codes[0], provider);
        double longitude = System.Convert.ToDouble(codes[1], provider);

        //save at singleton
        SingletonLatLng.instance.CoyoteLat.Add(latitude);
        SingletonLatLng.instance.CoyoteLng.Add(longitude);

        notifyManager(); //Call the Push notification function
    }

    //Functions that allow push notifications to appear within a mobile device
    public void notifyManager()
    {
        //Push notification setting
        var notificationParams = new NotificationParams
        {
            Id = UnityEngine.Random.Range(0, int.MaxValue),
            Delay = TimeSpan.FromSeconds(0.1),
            Title = "Coyote detected.",
            Message = "Coyote was detected on your farm.",
            Ticker = "Ticker",
            Sound = true,
            Vibrate = true,
            Light = true,
            SmallIcon = NotificationIcon.Bell,
            SmallIconColor = Color.red,
            LargeIcon = "app_icon"
        };

        //Send push notifications to display on mobile devices.
        NotificationManager.SendCustom(notificationParams);
    }

    //If the server is open show debug log open
    void ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("open");
    }
    //If the server is closed show debug log close
    void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close");
    }
}