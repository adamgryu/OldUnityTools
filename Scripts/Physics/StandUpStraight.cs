using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class StandUpStraight : MonoBehaviour {

    public enum UpVector {
        Up,
        Forward,
        Right,
    }

    public float standUpTorue = 10f;
    public UpVector upVector = UpVector.Up;
    private Rigidbody body;

    void Start() {
        this.body = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        Vector3 myUp = Vector3.up;
        switch (upVector) {
            case UpVector.Up: myUp = transform.up; break;
            case UpVector.Right: myUp = transform.right; break;
            case UpVector.Forward: myUp = transform.forward; break;
        }
        Vector3 torque = Vector3.Cross(myUp, Vector3.up) * this.standUpTorue;
        this.body.AddTorque(torque);
    }
}
