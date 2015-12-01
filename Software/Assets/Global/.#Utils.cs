using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class Utils : Singleton<Utils> {

	// Singleton
	protected Utils() {
		InputManager = new VInputManager ();
	}

	public enum Stations {Front, Right, Left};
	
	//InputManager
	public VInputManager InputManager{ get; private set; }
	public VInput Player1 { get { return InputManager.Player1; } }
	public VInput.AxisState Info = VInput.AxisState.Idle;
	private System.Random rng = new System.Random();
	public System.Random Rng {get{return rng;}}

	#region Static Varriables
	static public int Player1Id = 1;
	static public bool IsNetwork { get { return Network.isServer || Network.isClient; } }
	static public bool IsLocal { get { return !Utils.IsNetwork; } }
	#endregion
	
	#region Static Methods
	static public new UnityEngine.Object Instantiate (UnityEngine.Object prefab, Vector3 position, Quaternion rotation)
	{
		UnityEngine.Object instance = null;
			if (Utils.IsLocal) {
				instance = UnityEngine.Object.Instantiate (prefab, position, rotation);
			} else if (Network.isServer) {
					instance = Network.Instantiate (prefab, position, rotation, 0);
			}
			return instance;
	}

	static public void Destroy (GameObject obj){Destroy(obj,0f);}
	static public void Destroy (GameObject obj, float t)
	{
			if (Utils.IsLocal) {
				UnityEngine.Object.Destroy (obj,t);
			} else if (Network.isServer && obj != null && obj.networkView != null) {
				if(t > 0f && !GlobalScript.Instance.DestroyList.Any(x => x.Value == obj))
				{
					GlobalScript.Instance.DestroyList.Add(new TimedObject<GameObject>(obj,t));
				}
				else if(obj != null && obj.networkView != null)
				{
					//Debug.Log(string.Format("Destroying {0}",obj.name));
					Network.Destroy(obj);
				}
				
			}
	}


	static public void NetworkCommand (MonoBehaviour caller, string command, params System.Object[] parameters)
	{
		// By default, we use RPCMode.AllBuffered
		NetworkCommand (caller, command, RPCMode.AllBuffered, parameters);
	}

	static public void NetworkCommand (MonoBehaviour caller, string command, RPCMode rpcMode, params System.Object[] parameters)
	{
		if (Network.isServer && rpcMode == RPCMode.Server)
		{
			Type type = caller.GetType();
			MethodInfo method = type.GetMethod(command);
			method.Invoke(caller, parameters);
		}
		if (Network.isClient || Network.isServer) 
		{
			caller.networkView.RPC (command, rpcMode, parameters);
		} 
		else 
		{
			System.Type thisType = caller.GetType ();
			MethodInfo theMethod = thisType.GetMethod (command, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			theMethod.Invoke (caller, parameters);
		}
	}

	static public Vector3 SphereCoordinates (float theta, float phi, float radius)
	{
		float radPhi = Mathf.Deg2Rad * (90f - phi); // 90 - value to start from the bottom -- Drake
		float radTheta = Mathf.Deg2Rad * (theta + 180f); // add 180 degrees to have reference behind boat
		//http://fr.wikipedia.org/wiki/Coordonn%C3%A9es_sph%C3%A9riques
		return new Vector3 (
			radius * Mathf.Sin (radPhi) * Mathf.Sin (radTheta), 	//y
			radius * Mathf.Cos (radPhi),						//z
			radius * Mathf.Sin (radPhi) * Mathf.Cos (radTheta)	//x
				);
	}

	static public Vector3 HalfVector (Vector3 V1, Vector3 V2)
	{
		return (V1 + V2) / 2;
	}

	static public List<Vector3> RetrieveTrajectoryPoints(float speed, Vector3 direction, Vector3 initialPos, int iterations, float timeStep)
	{
		float currentTime = 0.0f;
		float force = speed;
		Vector3 startingPosition = initialPos;

		List<Vector3> points = new List<Vector3>(); 
		
		for (int i = 0 ; i < iterations ; i++)
			{
				float dx = force * direction.x * currentTime;
				float dy = force * direction.y * currentTime - (Physics2D.gravity.magnitude * currentTime * currentTime / 2.0f);
				float dz = force * direction.z * currentTime;
				Vector3 pos = new Vector3(startingPosition.x + dx, startingPosition.y + dy, startingPosition.z + dz);
				points.Add(pos);
				currentTime += timeStep;
			}

		return points;
	}

	static public void ReloadScene()
	{
		ScreenFader.Instance.SceneEnding = true;
		UIScript.Instance.resetInstance ();
		GlobalScript.setupImmediately = true;
		GlobalScript.Instance.resetInstance();
		Application.LoadLevel (Application.loadedLevelName);
	}

	static public void ReloadSceneAsync(bool setupImmediately)
	{
		ScreenFader.Instance.SceneEnding = true;
		UIScript.Instance.resetInstance ();
		ScreenFader.Instance.FadedOutCompletedEvent += () =>
		{
			GlobalScript.setupImmediately = setupImmediately;
			GlobalScript.Instance.LoadAsync(0);
		};
		//Application.LoadLevelAsync (0);

	}

	#endregion
}

