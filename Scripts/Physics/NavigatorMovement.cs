using UnityEngine;
using System.Collections;

public class NavigatorMovement : PhysicsMovement {

	public float successDistance = 0.7f;

	private Vector3? goal = null;
	protected NavMeshPath currentPath { get; private set; }
	protected int pathNode { get; private set; }
	private float recalculateTimer = 0.1f;

	// Use this to temporarily turn on and chasing. If false, the guy should just 
	// sit around like a lazy bum.
	public bool enableMovement = true;

	public bool hasGoal { get { return this.goal.HasValue; } }
	public bool hasValidPath { get { return this.currentPath != null && this.currentPath.status != NavMeshPathStatus.PathInvalid; } }
	public Vector3 destinationNode { get { return this.currentPath.corners[pathNode]; } }

	protected override void Update() {
		base.Update();

		if (this.enableMovement) {
			this.recalculateTimer -= Time.deltaTime;
			if (this.recalculateTimer <= 0) {
				this.OnCalculationTimeout();
				this.recalculateTimer = this.ResolveRecalculationTime(); // Distribute pathfinding more evenly.
			}

			if (this.hasGoal) {
				if (this.hasValidPath) {
					float distToGoal = (this.destinationNode - this.transform.position).SetY(0).magnitude;
					if (distToGoal < this.successDistance) {
						this.SelectNextNode();
					}
				} else if ((this.goal.Value - this.transform.position).SetY(0).magnitude < this.successDistance) {
					this.CompleteGoal();
				}
			}
		}
	}

	protected override void OnDrawGizmos() {
		base.OnDrawGizmos();
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

	protected virtual void OnCalculationTimeout() {
		if (this.hasGoal) {
			this.RecalculatePath();
		}
	}

	public void SetGoal(Vector3? goal) {
		this.goal = goal;
	}

	private void SelectNextNode() {
		this.pathNode++;
		if (this.pathNode >= this.currentPath.corners.Length) {
			this.CompleteGoal();
		}
	}

	protected virtual void CompleteGoal() {
		this.goal = null;
		this.currentPath = null;
	}


	protected void RecalculatePath() {
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(this.transform.position, goal.Value, int.MaxValue, path);
		this.currentPath = path;
		this.pathNode = Mathf.Min(1, path.corners.Length - 1);
	}

	public override Vector3 GetDesiredMovementDirection() {
		if (this.hasValidPath) {
			return (this.destinationNode - this.transform.position).SetY(0).normalized;
		}
		return Vector3.zero;
	}

	public virtual float ResolveSuccessDistance() {
		return successDistance;
	}

	protected virtual float ResolveRecalculationTime() {
		return Random.Range(0.1f, 0.5f);
	}
}
