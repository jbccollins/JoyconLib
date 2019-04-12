using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
	public Application.Bag ActiveBag;
	public Transform mainCamera;

	private List<Joycon> joycons;
    // Values made available via Unity
    public float[] stick;
    public int jc_ind = 0;
    private List<Rigidbody> thrownBags = new List<Rigidbody>();
    private int round;

	//private void reset () {
	//	mainCamera.transform.rotation = originalCameraRotation;
	//}

    void Start ()
    {
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
            Debug.LogError("No Joycon found");
		}
        ActiveBag = gameObject.AddComponent<Application.Bag>();
    }

	void FixedUpdate () {
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];
			if (j.GetButtonUp (Joycon.Button.SHOULDER_2))
			{
                ActiveBag.Throw(j);
			}
			if (j.GetButtonDown(Joycon.Button.DPAD_UP)) {
                ActiveBag.Reset();
            }
			if (j.GetButton(Joycon.Button.DPAD_LEFT)) {
                ActiveBag.Move(false);
			}
			if (j.GetButton(Joycon.Button.DPAD_RIGHT)) {
                ActiveBag.Move(true);
			}
			if (j.GetButtonDown(Joycon.Button.DPAD_DOWN)) {
                ActiveBag = gameObject.AddComponent<Application.Bag>();
            }
			// if (j.GetButton(Joycon.Button.STICK)) {
			// 	var stickX = j.GetStick()[0];
			// 	if (Mathf.Abs(stickX) > 0.2) {
			// 		rotateBag(stickX);
			// 	}
			// }
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
			var stickX = j.GetStick()[0];
			if (Mathf.Abs(stickX) > 0.2) {
                ActiveBag.Rotate(stickX);
			}
        }
    }
}
