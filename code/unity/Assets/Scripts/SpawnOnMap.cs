/* 
 * // Spawn On Map (Mapping) //
 * It is a code that generates pins to be mapped on a map and updates the mapped pins according to the state and movement of the map.
 * Lat means latitude, Lng means longitude.
 */

namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map; //Map prefab

		[SerializeField]
		Camera mainCamera;

		//Position value of sensor
		[Geocode]
		string[] _locationStrings = new string[3];
		Vector2d[] _locations;

		//Position value of coyote
		[Geocode]
		Vector2d[] _locationsCoyote;

		[SerializeField]
		float _spawnScale = 1f;

		//Sensor Prefab(Model) Pin
		[SerializeField]
		GameObject _markerPrefab;
		//Coyote Prefab(Model) Pin
		[SerializeField]
		GameObject _coyotePrefab;
		//Most recently detected Coyote prefab(Model) Pin
		[SerializeField]
		GameObject _newCoyotePrefab;

		//List of models that have already been spawn
		List<GameObject> _spawnedObjects;
		List<GameObject> _spawnedCoyoteObjects;

		// Use this for initialization
		void Start()
		{
			//Sensor Spawn
			_locations = new Vector2d[_locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Length; i++) //Loop as many sensors declared in the array
			{
				//Accessing sensor position values stored in Singleton
				var locationString = SingletonLatLng.instance.LatSensor[i] + "," + SingletonLatLng.instance.LngSensor[i];
				//Convert to a LatLon type that exists only in MapBox 
				_locations[i] = Conversions.StringToLatLon(locationString);

				//Sensor pin Spawn at the location on the map (Spawn info setting)
				var instance = Instantiate(_markerPrefab); 
				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance); //Add to the list of models that have already been spawn
			}

			//Coyote Spawn
			_locationsCoyote = new Vector2d[SingletonLatLng.instance.CoyoteLat.Count];
			_spawnedCoyoteObjects = new List<GameObject>();
			for (int i = 0; i < SingletonLatLng.instance.CoyoteLat.Count; i++) //Loop as many as the list of detected coyotes in Singleton stored
			{
				//Accessing coyote position values stored in Singleton
				var locationString = SingletonLatLng.instance.CoyoteLat[i] + "," + SingletonLatLng.instance.CoyoteLng[i];
				_locationsCoyote[i] = Conversions.StringToLatLon(locationString);

				//Coyote pin Spawn at the location on the map (Spawn info setting)
				var instance = Instantiate(_coyotePrefab);
				instance.transform.name = "CoyotePin" + i; //change spawned clone name
				instance.transform.localPosition = _map.GeoToWorldPosition(_locationsCoyote[i], true);
                instance.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				_spawnedCoyoteObjects.Add(instance); //Add to the list of models that have already been spawn
			}
        }

		// Update is called once per frame
		private void Update()
		{
			//Update the position and scale of the sensor pins(Clone) according to the map and environment situation
			int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }

			//Update the position and scale of the coyote pins(Clone) according to the map and environment situation
			int countCoyote = _spawnedCoyoteObjects.Count;
			for (int i = 0; i < countCoyote; i++)
			{
				//All models spawned so far convert from world position to local position, convert from world size to local size
				//if there is any additional coyote added to the list, calculate it again including that
				var spawnedCoyoteObject = _spawnedCoyoteObjects[i];
				Vector2d[] _locationsCoyoteUpdate = new Vector2d[SingletonLatLng.instance.CoyoteLat.Count]; //Detected coyote list count
				var locationString = SingletonLatLng.instance.CoyoteLat[i] + "," + SingletonLatLng.instance.CoyoteLng[i];
				_locationsCoyoteUpdate[i] = Conversions.StringToLatLon(locationString);
				var locationCoyote = _locationsCoyoteUpdate[i];
				spawnedCoyoteObject.transform.localPosition = _map.GeoToWorldPosition(locationCoyote, true);
				spawnedCoyoteObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			}

			//Additional spawn of detected coyotes in real-time, check the list every frame
			if (_spawnedCoyoteObjects.Count < SingletonLatLng.instance.CoyoteLat.Count)
			{
				//Spawn by setting the most recently detected Coyote position value as a reference
				var locationString = SingletonLatLng.instance.CoyoteLat[SingletonLatLng.instance.CoyoteLat.Count -1] + "," + SingletonLatLng.instance.CoyoteLng[SingletonLatLng.instance.CoyoteLat.Count - 1];
				var instance = Instantiate(_newCoyotePrefab);
				instance.transform.localPosition = _map.GeoToWorldPosition(Conversions.StringToLatLon(locationString), true);
				instance.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
				instance.transform.name = "CoyotePin" + _spawnedCoyoteObjects.Count; //Change clone pin name

				//Automatically move the camera to a real - time mapped location
				mainCamera.transform.localPosition = new Vector3(instance.transform.localPosition.x, 30, instance.transform.localPosition.z);
				_spawnedCoyoteObjects.Add(instance);
			}
		}
	}
}