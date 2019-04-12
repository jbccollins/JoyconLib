using System;
using UnityEngine;

namespace Application
{
    public class Bag : MonoBehaviour
    {
        private readonly float thrust = 200.0f;
        public Vector3 Gyro { get; set; }
        public Vector3 Accelleration { get; set; }
        public int jc_ind = 0;
        public Quaternion Orientation { get; set; }
        public Rigidbody Rigidbody { get; set; }
        private readonly Quaternion OriginalRotation = Quaternion.identity;
        private readonly Vector3 OriginalVelocity = Vector3.zero;
        private readonly Vector3 OriginalPosition = new Vector3(0, 2.0f, -15.0f);
        private Quaternion OriginalCameraRotation = Quaternion.Euler(new Vector3(20, 0, 0));
        public Bag()
        {
            Gyro = new Vector3(0, 0, 0);
            Accelleration = new Vector3(0, 0, 0);
        }

        private void Start()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.5f, 0.2f, 0.5f);
            cube.AddComponent<BoxCollider>();
            cube.AddComponent<Rigidbody>();

            //cube.GetComponent<BoxCollider>().material = BagPhysicsMaterial;
            Rigidbody = cube.GetComponent<Rigidbody>();
            Rigidbody.mass = 1.5f;
            Rigidbody.drag = 0;
            Rigidbody.angularDrag = 0.05f;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Reset();
        }

        public void Reset()
        {
            Rigidbody.useGravity = false;
            Rigidbody.transform.position = OriginalPosition;
            Rigidbody.transform.rotation = OriginalRotation;
            Rigidbody.velocity = OriginalVelocity;
            Rigidbody.angularVelocity = OriginalVelocity;
            //Rigidbody.transform.rotation = OriginalCameraRotation;
        }

        public void Throw(Joycon j)
        {
            Rigidbody.useGravity = true;
            //Debug.Log (transform.forward);
            Accelleration = j.GetAccel();
            // Joycon x axis is the same as Unity's y axis
            float upForce = System.Math.Abs(Accelleration.x);
            // Joycon z axis is the same as Unity's z axis
            float forwardForce = System.Math.Abs(Accelleration.z);
            Vector3 force = new Vector3(0, upForce, forwardForce) * thrust;
            Rigidbody.AddForce(force);
        }
        public void Move(bool left)
        {
            Vector3 newPosition = new Vector3(Rigidbody.transform.position.x + (0.05f * (left ? 1.0f : -1.0f)), Rigidbody.transform.position.y, Rigidbody.transform.position.z);
            Rigidbody.transform.position = newPosition;
        }
        public void Rotate(float stickX)
        {
            Debug.Log(string.Format("Stick x: {0:N}", stickX));
            // float speed = 1.0f * (stickX > 0 ? 1.0f : -1.0f);
            // Quaternion newBagRotation = Quaternion.Euler(new Vector3(bagRididBody.transform.rotation.x, bagRididBody.transform.rotation.y + speed, bagRididBody.transform.rotation.z));
            // Quaternion newCameraRotation = Quaternion.Euler(new Vector3(20, mainCamera.transform.rotation.y + speed, mainCamera.transform.rotation.z));

            //bagRididBody.transform.rotation = newBagRotation;
            Vector3 rot = Vector3.up * (stickX > 0 ? 1 : -1);
            Rigidbody.transform.Rotate(rot);
            //mainCamera.transform.Rotate(rot);
            //Follow();
        }
        // Position camera to follow behind player's head.
        //private void Follow(){

        //  // orientation as an angle when projected onto the XZ plane
        //  // this functionality is modularise into a separate method because
        //  // I use it elsewhere
        //  float playerAngle = AngleOnXZPlane (bagRididBody.transform);
        //  float cameraAngle = AngleOnXZPlane (mainCamera.transform);

        //  // difference in orientations
        //  float rotationDiff = Mathf.DeltaAngle(cameraAngle, playerAngle);

        //  // rotate around target by time-sensitive difference between these angles
        //  mainCamera.transform.RotateAround(bagRididBody.transform.position, Vector3.up, rotationDiff * Time.deltaTime);
        //}

        // Find the angle made when projecting the rotation onto the xz plane.
        // You could pass in the rotation as a parameter instead of the transform.
        private float AngleOnXZPlane(Transform item)
        {

            // get rotation as vector (relative to parent)
            Vector3 direction = item.rotation * new Vector3(1, 1, 1);

            // return angle in degrees when projected onto xz plane
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

    }
}
