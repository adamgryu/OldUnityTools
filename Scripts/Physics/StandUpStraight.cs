using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class StandUpStraight : MonoBehaviour {

    public float standUpTorue = 10f;
    private Rigidbody body;

    void Start() {
        this.body = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        Vector3 torque = Vector3.Cross(this.transform.up, Vector3.up) * this.standUpTorue;
        this.body.AddTorque(torque);

        // Stop the player from jiggling.
        if (Vector3.Dot(this.transform.up, Vector3.up) < 0.2f) {

        }
    }
}
