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
	private Quaternion originalRotation = Quaternion.identity;
	private Vector3 originalVelocity = Vector3.zero;
	private Vector3 originalPosition = new Vector3(0, 2.0f, -15.0f);

	private void reset () {
		rb.useGravity = false;
		gameObject.transform.position = originalPosition;
		gameObject.transform.rotation = originalRotation;
		rb.velocity = originalVelocity;
		rb.angularVelocity = originalVelocity;
	}

    void Start ()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;

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
				rb.useGravity = true;
				//Debug.Log (transform.forward);
				accel = j.GetAccel();
				
				Debug.Log (accel);
				// Joycon x axis is the same as Unity's y axis
				float upForce = System.Math.Abs(accel.x);
				// Joycon z axis is the same as Unity's z axis
				float forwardForce = System.Math.Abs(accel.z);
				Vector3 force = new Vector3(0, upForce, forwardForce) * thrust;
				rb.AddForce(force);
			}
			if (j.GetButtonDown(Joycon.Button.DPAD_UP)) {
				reset();
			}
		}
	}

    // Update is called once per frame
    void Update () {
		// make sure the Joycon only gets checked if attached
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];
			// GetButtonDown checks if a button has been pressed (not held)
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
				// Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
				j.Recenter ();
			}
        }
    }
}