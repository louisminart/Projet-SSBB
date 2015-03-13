using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	private float startTime;
	public Vector3 projection;
	
	void Start () {
		startTime = Time.time;
	}
	
	void Update () {
		if (Time.time - startTime > 3.0f) {
			Destroy (this.gameObject);
		}
	}
	
	void OnCollisionEnter(Collision coll)
	{
		GameObject target = coll.gameObject;
		if (target.tag=="Player") {
			Vector3 v = GetComponent<Rigidbody>().velocity;
			v+=projection;
			target.GetComponent<Rigidbody>().AddForce (v * 200);
		}
		
		Destroy (this.gameObject);	//quand un projectile touche quelque chose, delete
	}
	
}
