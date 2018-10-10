using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TriggerCollidersTracker<T> : MonoBehaviour {

    public int objectsInsideCount { get { return this.collidersInside.Count; } }
    protected List<Collider> collidersInside;

    protected virtual void Start() {
        this.collidersInside = new List<Collider>();
    }

    protected virtual void Update() {
        for (int i = collidersInside.Count - 1; i >= 0; i--) {
            var collider = collidersInside[i];
            if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy) {
                this.MarkColliderRemoved(collider);
            }
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.GetComponent<T>() == null) {
            return;
        }
        if (!this.collidersInside.Contains(collider)) {
            this.collidersInside.Add(collider);
            if (this.collidersInside.Count == 1) {
                this.OnAtLeastOneCollider();
            }
        }
        this.OnColliderEnter(collider);
    }

    private void OnTriggerExit(Collider collider) {
        this.MarkColliderRemoved(collider);
    }

    private void MarkColliderRemoved(Collider collider) {
        if (this.collidersInside.Contains(collider)) {
            this.collidersInside.Remove(collider);
            this.OnColliderExit(collider);
            if (this.collidersInside.Count == 0) {
                this.OnNoColliders();
            }
        }
    }

    protected virtual void OnAtLeastOneCollider() { }

    protected virtual void OnNoColliders() { }

    protected virtual void OnColliderEnter(Collider collider) { }

    protected virtual void OnColliderExit(Collider collider) { }
}

