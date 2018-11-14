using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BagController : MonoBehaviour {
	
	private List<Joycon> joycons;
	public float thrust = 200.0f;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;
	private Rigidbody rb;

    void Start ()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
		rb = GetComponent<Rigidbody>();

        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
			Destroy(gameObject);
		}
	}

	void FixedUpdate () {
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];
			if (j.GetButtonUp (Joycon.Button.SHOULDER_2))
			{
				Debug.Log ("Shoulder button 2 released");
				//Debug.Log (transform.forward);
				accel = j.GetAccel();
				
				Debug.Log (accel);
				// Joycon x axis is the same as Unity's y axis
				float upForce = System.Math.Abs(accel.x);
				// Joycon z axis is the same as Unity's z axis
				float forwardForce = System.Math.Abs(accel.z);
				float sidewaysForce = accel.y;
				Vector3 force = new Vector3(0, upForce, forwardForce) * thrust;
				rb.AddForce(force);
			}
		}
	}

    // Update is called once per frame
    void Update () {
		if ( Input.GetKeyDown ( KeyCode.P ) ) {
			Debug.Log("P PRESSED");
			//int scene = SceneManager.GetActiveScene().buildIndex;
			//SceneManager.LoadScene(scene, LoadSceneMode.Single);
			// Use a coroutine to load the Scene in the background
            StartCoroutine(LoadYourAsyncScene());
		}
		// make sure the Joycon only gets checked if attached
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];
			if (j.GetButtonDown(Joycon.Button.DPAD_UP)) {
				// Restart the level
				Debug.Log(SceneManager.GetActiveScene ().name);
				//SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				return;
			}
			// GetButtonDown checks if a button has been pressed (not held)
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
				Debug.Log ("Shoulder button 2 pressed");
				// GetStick returns a 2-element vector with x/y joystick components
				Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}",j.GetStick()[0],j.GetStick()[1]));
            
				// Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
				j.Recenter ();
			}
			// GetButtonDown checks if a button is currently down (pressed or held)
			if (j.GetButton (Joycon.Button.SHOULDER_2))
			{
				Debug.Log ("Shoulder button 2 held");
			}

			if (j.GetButtonDown (Joycon.Button.DPAD_DOWN)) {
				Debug.Log ("Rumble");

				// Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
				// https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

				j.SetRumble (160, 320, 0.6f, 200);

				// The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
                // (Useful for dynamically changing rumble values.)
				// Then call SetRumble(0,0,0) when you want to turn it off.
			}
        }
    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Testing");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}