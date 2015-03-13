using UnityEngine;
using System.Collections;

public class ParticleTimer : MonoBehaviour {

	float time =0;
	// Use this for initialization
	void Start () {
		time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - time > 0.5) {
			Destroy (this.gameObject);
		}
	}
}
