using UnityEngine;
using System.Collections;

public class Animation : MonoBehaviour {

	private bool jumping=false;
	private Animator anim;

	// Use this for initialization
	void Start () {
		anim=GetComponent<Animator>();
		transform.rotation = Quaternion.AngleAxis (90, Vector3.up);
	}
	
	// indentation de MERDE
	void Update () {
		if (!anim.GetCurrentAnimatorStateInfo (0).IsName ("Armature_Hit")) {
						float x = GetComponent<Rigidbody>().velocity.x;
						if (GetComponent<Rigidbody>().velocity.y > 0.1f) {
								jumping = true;
						}
						if (x > 0.01f) {
								transform.rotation = Quaternion.AngleAxis (-90, Vector3.up);
						}
						if (x < -0.01f) {
								transform.rotation = Quaternion.AngleAxis (90, Vector3.up);
						}
				
						if (Mathf.Abs (x) > 1.0f && !jumping) {
								anim.Play ("Armature_Run");
						} else if (jumping) {
								anim.Play ("Armature_Jump");
						} else {
								anim.Play ("Armature_Idle");
						}
				}
		}


	void OnCollisionStay(Collision coll)
	{
		jumping = false;
	}


	public void animAttack()
	{
		anim.Play("Armature_Hit", -1, 0f);
	}


}
