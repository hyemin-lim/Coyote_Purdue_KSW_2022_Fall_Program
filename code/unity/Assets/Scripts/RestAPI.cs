/* 
 * // HttpWebRequest REST-API //
 * The Node.Js server in the Raspberry pie must be turned on to access it.
 * Code to retrieve data from the server for up to five locations of the last detected coyote stored in the DB and sensor locations.
 * Lat means latitude, Lng means longitude.
 * 
 * http://192.168.2.222:8081/api/sensors/getSound1Coord
 * Data received example:
 * {
 *   "id": "sound1",
 *   "x": "40.4203008430482",
 *   "y": "-86.90254211425781"
 * }
 * 
 * http://192.168.2.222:8081/api/coyotes/getInitialCoyotes
 * Data received example:
 * {
 *   "msg": [
 *      "40.4274008430482/-86.94259991423781",
 *      "40.4273008430482/-86.94299991423781",
 *      "40.4275008430482/-86.94399991423781",
 *      "40.4274008430482/-86.94359991423781",
 *      "40.4274008430482/-86.94359991423781"
 *  ]
 * }
 * 
 */

using System;
using System.IO;
using System.Net;
using UnityEngine;
using System.Globalization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestAPI : MonoBehaviour
{
    public Transform FailedLoadPanel;
    public Text FailedText;
    private bool failed = false;

    //Start event function invokes once between Awake and Update function call when component is active
    void Start()
    {
        //Loop as many sensors
        for (int i= 1; i<4; i++)
        {
            //Only the number of sensors in url can be changed to receive data from the corresponding sensor.
            string url = "http://YourServerIP:PORT/api/sensors/getSound" + i + "Coord"; //Insert Your Server IP, PORT
            GetAllSensorLatLng(url, i); //The function's parameter are the URL and Sensor Number to use for WebRequest
        }
        GetAllCoyoteHistory("http://YourServerIP:PORT/api/coyotes/getInitialCoyotes"); //Insert Your Server IP, PORT

        //If there is no problem with all HttpWebrequests, switch to MapScene
        if (failed == false)
            SceneManager.LoadScene("Coyote");
    }

    //The function that receives position values from the server for all Coyotes (up to 5) stored in the DB
    public void GetAllCoyoteHistory(string sendurl)
    {
        //Address to send request & setting
        HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(sendurl)) as HttpWebRequest;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";

        //Convert string to byte[]
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes("");
        httpWebRequest.ContentLength = (long)bytes.Length;

        //Send data in stream format
        using (Stream requestStream = httpWebRequest.GetRequestStream())
            requestStream.Write(bytes, 0, bytes.Length);

        string result = null;
        try
        {
            //Receive response data in Stream Reader
            using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
                result = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();

            //Trim received data with Split
            string[] codes = result.Split('[');
            string[] code = codes[1].Split(']');
            string[] splitCodes = code[0].Split('"');

            //Cheack how many data has come through the split array
            int dataLength = 0;
            if (splitCodes[3] == null)
                dataLength = 1;
            else if (splitCodes[5] == null)
                dataLength = 2;
            else if (splitCodes[7] == null)
                dataLength = 3;
            else if (splitCodes[9] == null)
                dataLength = 4;
            else if (splitCodes[9] != null)
                dataLength = 5;

            SaveToSingleton(splitCodes, dataLength); //Split completed data and data length are parameters of the function.


        }
        catch (WebException e)
        {
            //Change the bool value so that do not move on to the next scene when the request is fail
            failed = true;
            Debug.Log(e.Message);

            //Shows UI that failed to load data.
            if (FailedLoadPanel.gameObject.activeSelf == false) //If the UI already exists, prevent the UI from appearing in duplicate.
            {
                FailedLoadPanel.gameObject.SetActive(true);
                FailedText.text = "Failed to load. \n" + e.Message;
            }

        }
        catch (Exception e)
        {
            failed = true;
            Debug.Log("Failed to load: " + e.Message);
            if (FailedLoadPanel.gameObject.activeSelf == false)
                FailedLoadPanel.gameObject.SetActive(true);
        }
    }

    public void GetAllSensorLatLng(string sendurl, int sensorNumber)
    {
        HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(sendurl)) as HttpWebRequest;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";

        string msg = "";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msg);
        httpWebRequest.ContentLength = (long)bytes.Length;

        using (Stream requestStream = httpWebRequest.GetRequestStream())
            requestStream.Write(bytes, 0, bytes.Length);

        string result = null;
        try
        {
            using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
                result = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();

            //Trim received data with Split
            string[] splitResult = result.Split('"');
            string lat = splitResult[7];
            string lng = splitResult[11];

            //Converts a specified value to a double-precision floating-point number.
            //Provides specific information for formatting and parsing numeric values.
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = "."; //Displays the same value with a blank as the separator.
            double latitude = System.Convert.ToDouble(lat, provider);
            double longitude = System.Convert.ToDouble(lng, provider);

            //Call a function at SingletonLatLng.cs
            SingletonLatLng.instance.AddLatLng(latitude, longitude, sensorNumber);

        }
        catch (WebException e)
        {
            failed = true;
            Debug.Log(e.Message);
            if (FailedLoadPanel.gameObject.activeSelf == false)
            {
                FailedLoadPanel.gameObject.SetActive(true);
                FailedText.text = "Failed to load. \n" + e.Message;
            }

        }
        catch (Exception e)
        {
            failed = true;
            Debug.Log("Failed to load: " + e.Message);
            if (FailedLoadPanel.gameObject.activeSelf == false)
                FailedLoadPanel.gameObject.SetActive(true);
        }
    }

    public void SaveToSingleton(string[] splitcodes, int count)
    {
        NumberFormatInfo provider = new NumberFormatInfo();
        provider.NumberDecimalSeparator = ".";

        for (int i = 0; i < count; i++)
        {
            //To find data in an array
            //Calculated according to the rule below is num
            /* i = 0, num = 1
             * i = 1, num = 3
             * i = 2, num = 5
             * i = 3, num = 7
             * i = 4, num = 9*/
            int num = 2 * i + 1;

            //Split the '/' dividing latitude and longitude
            string[] coyoteDataSplited = splitcodes[num].Split('/');
            //Convert the string value to a double value to access the list variable in the singletone and add it
            SingletonLatLng.instance.CoyoteLat.Add(System.Convert.ToDouble(coyoteDataSplited[0], provider));
            SingletonLatLng.instance.CoyoteLng.Add(System.Convert.ToDouble(coyoteDataSplited[1], provider));
        }
    }
}
