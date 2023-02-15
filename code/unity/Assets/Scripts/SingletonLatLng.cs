/* 
 * // Singleton Pattern //
 * Code to store data from servers and access it from anywhere at any time.
 * Declares a variable called instance statically so that it can be retrieved from scripts within other objects.
 * Lat means latitude, Lng means longitude.
 */

using UnityEngine;
using System.Collections.Generic;

public class SingletonLatLng : MonoBehaviour
{
    public static SingletonLatLng instance = null;

    //Positions of the three sensors
    public double[] LatSensor = new double[3];
    public double[] LngSensor = new double[3];

    //Last detected coyote positions list
    public List<double> CoyoteLat = new List<double>();
    public List<double> CoyoteLng = new List<double>();

    //Awake is called only once in each script and only after another object is initialized
    void Awake()
    {
        if (instance == null) //The instance is null, that is, it does not exist on the system
        { 
            instance = this; //put this(class) in an instance.
            DontDestroyOnLoad(gameObject); //OnLoad Keeps this object Undestroyed
        } 
        else 
        { 
            if (instance != this) //If the instance is not this class, it means that there is already have one instance
                Destroy(this.gameObject); //Delete this object that has just been awaked because more than one object should not exist
        }

        //List initialization and Clear
        CoyoteLat.Clear();
        CoyoteLng.Clear();
    }

    //Global functions that are accessible elsewhere when accessed through a single-ton instance
    //Adds the value of the sensor array.
    public void AddLatLng(double lat, double lng, int sensorNum)
    {
        LatSensor.SetValue(lat, sensorNum-1); //sensorNum-1 because the value of the factor starts from 1 and the array starts from 0
        LngSensor.SetValue(lng, sensorNum-1);
    }
}