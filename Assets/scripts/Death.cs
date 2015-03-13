using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

	public GameObject part;

	// Use this for initialization
	void Start () {
	}


	void OnCollisionStay(Collision coll)
	{
		coll.gameObject.SetActive (false);
		Vector3 vect = new Vector3 (coll.contacts[0].point.x,coll.contacts[0].point.y,-6);

		GameObject particle = Instantiate (part, vect, Quaternion.identity) as GameObject;

		//var distanceToPlane = Vector3.Dot(new Vector3 (0, 1, 0), new Vector3(0,0,-6) - particle.position);
		//var planePoint = new Vector3 (0, 0, -6) - new Vector3 (0, 1, 0) * distanceToPlane;
		Vector3 position = particle.transform.position;
		position.z += 5;
		particle.transform.position=position;
		particle.transform.LookAt(new Vector3 (0, 0, 5) - particle.transform.position);

	}

}
