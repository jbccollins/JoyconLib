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
        }

        void Start()
        {
            Gyro = new Vector3(0, 0, 0);
            Accelleration = new Vector3(0, 0, 0);
            Rigidbody = new Rigidbody();
            Reset();
        }

        private void Reset()
        {
            Rigidbody.useGravity = false;
            Rigidbody.transform.position = OriginalPosition;
            Rigidbody.transform.rotation = OriginalRotation;
            Rigidbody.velocity = OriginalVelocity;
            Rigidbody.angularVelocity = OriginalVelocity;
            Rigidbody.transform.rotation = OriginalCameraRotation;
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

    }
}
