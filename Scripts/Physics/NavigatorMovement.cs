using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;

/// <summary>
/// A physics movement rigidbody that follows the nav mesh.
/// </summary>
public class NavigatorMovement : PhysicsMovement {

    private NavMeshNavigator navigator;

    private void Start() {
        navigator = gameObject.AddComponent<NavMeshNavigator>();
    }

    public void SetGoal(Vector3 goal) {
        navigator.SetGoal(goal);
    }

    public override Vector3 GetDesiredMovementVector() {
        return navigator.GetMovementDirection();
    }
}

/// <summary>
/// Uses the nav mesh to calculate the direction the object should move.
/// This direction can be retrieved by calling <see cref="GetMovementDirection()"/>.
/// </summary>
public class NavMeshNavigator : MonoBehaviour {
    public float successDistance = 0.7f;
    public bool enableMovement = true;
    public RandomRange repathTimeout = new RandomRange(0.25f, 1f);

    public bool hasGoal { get { return this.goal.HasValue; } }
    public bool hasValidPath { get { return this.currentPath != null && this.currentPath.status != UnityEngine.AI.NavMeshPathStatus.PathInvalid; } }
    public Vector3 destinationNode { get { return this.currentPath.corners[pathNode]; } }
    public Vector3? goal { get; private set; }
    public event Action onGoalReached;
    public event Action onBeforeCalculationTimeout;

    protected NavMeshPath currentPath { get; private set; }
    protected int pathNode { get; private set; }
    private float recalculateTimer = 0.1f;
    private NavMeshPath cachedReusablePath;

    private void Awake() {
        cachedReusablePath = new NavMeshPath();
    }

    protected virtual void Update() {
        if (this.enableMovement) {
            this.recalculateTimer -= Time.deltaTime;
            if (this.recalculateTimer <= 0) {
                this.OnCalculationTimeout();
                this.recalculateTimer = this.GetRecalculationTime(); // Distribute pathfinding more evenly.
            }

            if (this.hasGoal) {
                if (this.hasValidPath) {
                    float distToGoal = (this.destinationNode - this.transform.position).SetY(0).magnitude;
                    if (distToGoal < this.successDistance) {
                        this.SelectNextNode();
                    }
                } else if ((this.goal.Value - this.transform.position).SetY(0).magnitude < this.successDistance) {
                    this.ReachGoal();
                }
            }
        }
    }

    protected virtual void OnCalculationTimeout() {
        if (onBeforeCalculationTimeout != null) {
            onBeforeCalculationTimeout();
        }
        if (this.hasGoal) {
            this.RecalculatePath();
        }
    }

    /// <summary>
    /// Returns the direction this object should move to follow the path.
    /// </summary>
    public Vector3 GetMovementDirection() {
        if (this.hasValidPath) {
            return (this.destinationNode - this.transform.position).SetY(0).normalized;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Paths to the given goal on the goal update event.
    /// </summary>
	public void SetGoal(Vector3 goal) {
        this.goal = goal;
    }

    /// <summary>
    /// Paths to the given goal and immediately repaths.
    /// </summary>
    public void SetGoalImmediately(Vector3 goal) {
        SetGoal(goal);
        RecalculatePath();
    }

    public void ClearGoal() {
        this.goal = null;
        this.currentPath = null;
    }

    private void ReachGoal() {
        ClearGoal();
        if (onGoalReached != null) {
            onGoalReached();
        }
    }

    public float GetDistanceToGoal() {
        if (!hasValidPath) {
            return 0;
        }

        float distance = (transform.position - destinationNode).magnitude;
        for (int i = pathNode; i < currentPath.corners.Length - 1; i++) {
            distance += (currentPath.corners[i + 1] - currentPath.corners[i]).magnitude;
        }
        return distance;
    }

    private void SelectNextNode() {
        this.pathNode++;
        if (this.pathNode >= this.currentPath.corners.Length) {
            this.ReachGoal();
        }
    }

    private void RecalculatePath() { 
        NavMesh.CalculatePath(this.transform.position, goal.Value, int.MaxValue, cachedReusablePath);
        this.currentPath = cachedReusablePath;
        this.pathNode = Mathf.Min(1, currentPath.corners.Length - 1);
    }

    private float GetRecalculationTime() {
        return repathTimeout.Random();
    }

    protected virtual void OnDrawGizmos() {
        if (this.hasValidPath) {
            for (int i = 1; i < this.currentPath.corners.Length; i++) {
                Gizmos.DrawLine(this.currentPath.corners[i - 1], this.currentPath.corners[i]);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.destinationNode, this.successDistance);
        }

        if (this.hasGoal) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(this.goal.Value, Vector3.one * 1.4f);
        }
    }
}