using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This class applies forces to a rigidbody so that its movement mimics that
/// of a character controller.
/// </summary>
public abstract class PhysicsMovement : MonoBehaviour {

    const float DESIRED_DELTA_TIME = 1 / 60f;
    const float EPSILON = 0.001f;

    /// <summary>
    /// The movement velocity of the character that we try to reach.
    /// </summary>
    public float maxVelocity = 5;

    /// <summary>
    /// The strength of the forces applied to the character to reach the maximum velocity.
    /// </summary>
    public float movementForce = 10;

    /// <summary>
    /// The strength of the movement force when suspended in midair.
    /// </summary>
    public float midairForce = 5;

    /// <summary>
    /// Additional gravity force to be applied to the character while it is grounded.
    /// </summary>
    public float groundStickyForce = 1f;

    /// <summary>
    /// The length of the ray that is shot beneath the character to determine if it is grounded.
    /// </summary>
    public float midairRaycastDistance = 0.2f;

    /// <summary>
    /// The offset of the ray from the bottom of the collider that determines if it is grounded.
    /// </summary>
    public float midairRaycastBottomOffset = -0.3f;

    /// <summary>
    /// The size of the sphere-cast that is shot to determine if it is grounded.
    /// </summary>
    public float midairRayRadius = 0.5f;

    /// <summary>
    /// The speed at which the object turns to face its direction.
    /// </summary>
    public float turnSpeed = 4;

    /// <summary>
    /// The mask used for the grounding raycast.
    /// </summary>
    public LayerMask groundRaycastMask;

    /// <summary>
    /// The position for the force is applied. If left null, it uses the center.
    /// </summary>
    public GameObject forceApplyPosition;

    /// <summary>
    /// The friction value of the physics material while standing still on the ground.
    /// </summary>
    public float standingFriction = 1f;

    /// <summary>
    /// The friction value while mid-air or moving itself.
    /// </summary>
    public float movingFriction = 0f;

    /// <summary>
    /// A cached value of CheckIfGrounded that is updated at the beginning of each update loop.
    /// </summary>
    public bool isGrounded { get; protected set; }

    /// <summary>
    /// Disables physics material friction entirely.
    /// </summary>
    public bool disableFriction { get; set; }

    /// <summary>
    /// The direction that points up from the character's head.
    /// </summary>
    public Vector3 upDirection {
        get { return -Physics.gravity.normalized; }
    }

    /// <summary>
    ///  A reference to the rigidbody for this object.
    /// </summary>
    public Rigidbody body { get; protected set; }

    /// <summary>
    /// A reference to the collider for this object.
    /// </summary>
    protected Collider myCollider;

    /// <summary>
    /// The position where the ground raycast starts.
    /// </summary>
    private Vector3 groundRaycastStart { get { return myCollider.bounds.center + Vector3.down * (myCollider.bounds.extents.y + midairRaycastBottomOffset); } }


    #region UnityEvents

    protected virtual void Awake() {
        this.isGrounded = true;
        this.body = this.GetComponent<Rigidbody>();
        this.myCollider = this.GetComponent<Collider>();
        if (this.forceApplyPosition == null) {
            this.forceApplyPosition = this.gameObject;
        }
    }

    protected virtual void Update() {
        // Override me.
    }

    protected virtual void FixedUpdate() {
        this.UpdateMovement();

        // Lets the object stay rooted to the ground when standing.
        if (this.isGrounded) {
            this.body.AddForce(-this.transform.up * this.groundStickyForce);
        }

        // Stop moving if we close to stopping.
        if (this.body.velocity.SetY(0).sqrMagnitude <= EPSILON * EPSILON) {
            this.body.velocity = this.body.velocity.SetX(0).SetZ(0);
        }

        // Stop sliding on the ground by applying friction sometimes.
        bool noInput = GetDesiredMovementDirection() * ResolveMaximumVelocity() == Vector3.zero;
        float friction = (isGrounded && noInput && !disableFriction) ? standingFriction : 0f;
        myCollider.material.dynamicFriction = friction;
        myCollider.material.staticFriction = friction;
    }

    protected virtual void OnDrawGizmos() {
        if (!myCollider) {
            this.myCollider = GetComponent<Collider>();
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundRaycastStart, groundRaycastStart + Vector3.down * this.midairRaycastDistance);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundRaycastStart + Vector3.down * this.midairRaycastDistance, this.midairRayRadius);
    }

    #endregion

    #region BehaviourLogic

    protected virtual void UpdateMovement() {
        this.isGrounded = this.CheckIfGrounded();
        Vector3 direction = this.GetDesiredMovementDirection();

        if (this.ResolveCanMove()) {
            Vector3 desiredVelocity = direction * this.ResolveMaximumVelocity();
            Vector3 currentVelocity = this.GetComponent<Rigidbody>().velocity.SetY(0); // TODO: Generalize to allow movement along different planes.
            Vector3 diff = desiredVelocity - currentVelocity;
            float force = this.ResolveMovementForce();
            this.body.AddForceAtPosition(diff * force * Time.fixedDeltaTime, this.forceApplyPosition.transform.position);
        }

        Vector3 rotateDirection = this.GetDesiredRotateDirection();
        if (this.ResolveCanRotate() && rotateDirection != Vector3.zero) {
            float angle = Vector3.Angle(this.GetRotationModel().transform.forward.SetY(0), rotateDirection)
                * Mathf.Sign(Vector3.Cross(this.GetRotationModel().transform.forward.SetY(0), rotateDirection).y);
            this.GetRotationModel().transform.Rotate(this.upDirection, angle * this.turnSpeed * Time.fixedDeltaTime);
        }
    }

    public virtual bool CheckIfGrounded() {
        RaycastHit hit;
        bool result = Physics.SphereCast(new Ray(groundRaycastStart, Vector3.down), this.midairRayRadius, out hit, this.midairRaycastDistance, this.groundRaycastMask.value);
        return result;
    }

    #endregion

    #region OverrideBehaviour

    public abstract Vector3 GetDesiredMovementDirection();

    protected virtual Vector3 GetDesiredRotateDirection() {
        return this.GetDesiredMovementDirection();
    }

    protected virtual bool ResolveCanRotate() {
        return this.ResolveCanMove() && this.isGrounded;
    }

    protected virtual GameObject GetRotationModel() {
        return this.gameObject;
    }

    protected virtual bool ResolveCanMove() {
        return true;
    }

    public virtual float ResolveMovementForce() {
        float force = this.isGrounded ? this.movementForce : this.midairForce;
        return force;
    }

    public virtual float ResolveMaximumVelocity() {
        return this.maxVelocity;
    }

    #endregion
}