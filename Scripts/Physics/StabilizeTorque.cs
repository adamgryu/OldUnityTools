using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class StabilizeTorque : MonoBehaviour {

    public Vector3 upDirection;
    public float upTorque;

    public Vector3 forwardDirection;
    public float forwardTorque;

    private Rigidbody body;

    void Start() {
        this.body = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        Vector3 torque = Vector3.Cross(this.transform.up, upDirection) * this.upTorque;
        this.body.AddTorque(torque);
        torque = Vector3.Cross(this.transform.forward, forwardDirection) * this.forwardTorque;
        this.body.AddTorque(torque);
    }
}
