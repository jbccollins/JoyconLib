using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
	public Transform activeBag;
	public Transform mainCamera;

	private List<Joycon> joycons;
	public float thrust = 200.0f;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;
	private Rigidbody bagRididBody;
	private Quaternion originalRotation = Quaternion.identity;
	private Vector3 originalVelocity = Vector3.zero;
	private Vector3 originalPosition = new Vector3(0, 2.0f, -15.0f);
	private Quaternion originalCameraRotation = Quaternion.Euler(new Vector3(20, 0, 0));
    private List<Rigidbody> thrownBags = new List<Rigidbody>();

	private void reset () {
		bagRididBody.useGravity = false;
		bagRididBody.transform.position = originalPosition;
		bagRididBody.transform.rotation = originalRotation;
		bagRididBody.velocity = originalVelocity;
		bagRididBody.angularVelocity = originalVelocity;
		mainCamera.transform.rotation = originalCameraRotation;
	}

    private void CloneBag () {
        thrownBags.Add(bagRididBody);
        Rigidbody newBagRigidBody = Instantiate(bagRididBody);
        newBagRigidBody.useGravity = false;
        newBagRigidBody.transform.position = originalPosition;
        newBagRigidBody.transform.rotation = originalRotation;
        newBagRigidBody.velocity = originalVelocity;
        newBagRigidBody.angularVelocity = originalVelocity;
        mainCamera.transform.rotation = originalCameraRotation;
        bagRididBody = newBagRigidBody;
    }

	private void throwBag (Joycon j) {
		bagRididBody.useGravity = true;
		//Debug.Log (transform.forward);
		accel = j.GetAccel();
		
		Debug.Log (accel);
		// Joycon x axis is the same as Unity's y axis
		float upForce = System.Math.Abs(accel.x);
		// Joycon z axis is the same as Unity's z axis
		float forwardForce = System.Math.Abs(accel.z);
		Vector3 force = new Vector3(0, upForce, forwardForce) * thrust;
		bagRididBody.AddForce(force);
	}

	private void moveBag(bool left) {
		Vector3 newPosition = new Vector3(bagRididBody.transform.position.x + (0.05f * (left ? 1.0f : -1.0f)), bagRididBody.transform.position.y, bagRididBody.transform.position.z);
		bagRididBody.transform.position = newPosition;
	}

	// Position camera to follow behind player's head.
	private void Follow(){

		// orientation as an angle when projected onto the XZ plane
		// this functionality is modularise into a separate method because
		// I use it elsewhere
		float playerAngle = AngleOnXZPlane (bagRididBody.transform);
		float cameraAngle = AngleOnXZPlane (mainCamera.transform);

		// difference in orientations
		float rotationDiff = Mathf.DeltaAngle(cameraAngle, playerAngle);

		// rotate around target by time-sensitive difference between these angles
		mainCamera.transform.RotateAround(bagRididBody.transform.position, Vector3.up, rotationDiff * Time.deltaTime);
	}

	// Find the angle made when projecting the rotation onto the xz plane.
	// You could pass in the rotation as a parameter instead of the transform.
	private float AngleOnXZPlane(Transform item){

		// get rotation as vector (relative to parent)
		Vector3 direction = item.rotation * new Vector3(1, 1, 1);

		// return angle in degrees when projected onto xz plane
		return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
	}

	private void rotateBag(float stickX) {
		Debug.Log(string.Format("Stick x: {0:N}",stickX));
		// float speed = 1.0f * (stickX > 0 ? 1.0f : -1.0f);
		// Quaternion newBagRotation = Quaternion.Euler(new Vector3(bagRididBody.transform.rotation.x, bagRididBody.transform.rotation.y + speed, bagRididBody.transform.rotation.z));
		// Quaternion newCameraRotation = Quaternion.Euler(new Vector3(20, mainCamera.transform.rotation.y + speed, mainCamera.transform.rotation.z));
		
		//bagRididBody.transform.rotation = newBagRotation;
		Vector3 rot = Vector3.up * (stickX > 0 ? 1 : -1);
		bagRididBody.transform.Rotate(rot);
		//mainCamera.transform.Rotate(rot);
		Follow();
	}

    void Start ()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
		bagRididBody = activeBag.GetComponent<Rigidbody>();
		bagRididBody.useGravity = false;

        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
			Destroy(gameObject);
		}
		reset();
	}

	void FixedUpdate () {
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];
			if (j.GetButtonUp (Joycon.Button.SHOULDER_2))
			{
				throwBag(j);
			}
			if (j.GetButtonDown(Joycon.Button.DPAD_UP)) {
                //reset();
                Debug.Log("DPAD_UP");
                CloneBag();

            }
			if (j.GetButton(Joycon.Button.DPAD_LEFT)) {
				moveBag(false);
			}
			if (j.GetButton(Joycon.Button.DPAD_RIGHT)) {
				moveBag(true);
			}
			if (j.GetButtonDown(Joycon.Button.DPAD_DOWN)) {
				Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}",j.GetStick()[0],j.GetStick()[1]));
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
				rotateBag(stickX);
			}
        }
    }
}
