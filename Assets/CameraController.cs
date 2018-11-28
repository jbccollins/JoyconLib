using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Transform target;
	public float distanceBehind;
	public float distanceAbove;

	// Use this for initialization
	void Start ()     {

	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = new Vector3(target.position.x, target.position.y + distanceAbove, target.position.z - distanceBehind);
    }
}
