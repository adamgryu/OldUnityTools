using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a specified number of particles and cleans up the object after.
/// 
/// NOTE: This will override Emission settings on the system.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class CleanUpParticles : MonoBehaviour {

    private ParticleSystem system;

	void Start () {
        this.system = this.GetComponent<ParticleSystem>();
    }
	
	void Update () {
		if (!this.system.IsAlive()) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
