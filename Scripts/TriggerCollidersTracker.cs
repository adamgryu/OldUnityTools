using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TriggerCollidersTracker<T> : MonoBehaviour {

    public int objectsInsideCount { get { return this.collidingColliders.Count; } }
    protected List<Collider> collidingColliders;

    protected virtual void Start() {
        this.collidingColliders = new List<Collider>();
    }

    protected virtual void Update() {
        this.collidingColliders.BufferedForEach(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy, this.MarkColliderRemoved);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.GetComponent<T>() == null) {
            return;
        }
        if (!this.collidingColliders.Contains(collider)) {
            this.collidingColliders.Add(collider);
            if (this.collidingColliders.Count == 1) {
                this.OnAtLeastOneCollider();
            }
        }
        this.OnColliderEnter(collider);
    }

    private void OnTriggerExit(Collider collider) {
        this.MarkColliderRemoved(collider);
    }

    private void MarkColliderRemoved(Collider collider) {
        if (this.collidingColliders.Contains(collider)) {
            this.collidingColliders.Remove(collider);
            if (collider != null) {
                this.OnColliderExit(collider);
            }
            if (this.collidingColliders.Count == 0) {
                this.OnNoColliders();
            }
        }
    }

    protected virtual void OnAtLeastOneCollider() { }

    protected virtual void OnNoColliders() { }

    protected virtual void OnColliderEnter(Collider collider) { }

    protected virtual void OnColliderExit(Collider collider) { }
}

