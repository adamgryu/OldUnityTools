using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a specified number of particles and cleans up the object after.
/// 
/// NOTE: This will override Emission settings on the system.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticleBurst : MonoBehaviour {

	public int burstSize = 50;
    private ParticleSystem system;

	void Start () {
        this.system = this.GetComponent<ParticleSystem>();
        this.system.Emit(burstSize);
    }
	
	void Update () {
		if (this.system.particleCount <= 0) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
