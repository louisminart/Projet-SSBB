using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private Network networkScript;
	private Animator anim;
	public GameObject network;
	public GameObject Eclair;
	public GameObject FatalFoudre;

	private float lastAttack=0;



	void Start () {
		networkScript = network.GetComponent<Network> ();
		anim=GetComponent<Animator>();
		jumpLeft = 2;
	}


	private int jumpLeft;

	private bool jumping=false;
	private bool running=false;

	// Update is called once per frame
	void FixedUpdate () {
		move ();
		attack ();
		networkScript.sendMove ();
		setAnimation ();
	}


	private void setAnimation()
	{
		if (!anim.GetCurrentAnimatorStateInfo (0).IsName("Armature_Hit")) {
						if (jumping) {
						
						} else if (running) {
								anim.Play ("Armature_Run");
						} else {
								anim.Play ("Armature_Idle");
						}
				}
	}


	void OnCollisionEnter(Collision coll)
	{
		jumping = false;
		jumpLeft = 2;
	}

	void OnCollisionExit(Collision coll)
	{
		jumping = true;
	}











	private void eclair()
	{
		anim.Play("Armature_Hit", -1, 0f);
		Vector3 position = transform.position;
		position = new Vector3 (position.x, position.y+0.5f, position.z);
		GameObject eclair = Instantiate (Eclair, position, Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(GetComponent<Collider>(), eclair.GetComponent<Collider>());
		float x = transform.rotation.y < 0 ? 10 : -10;
		Vector3 v = new Vector3 (x, 0, 0);
		eclair.GetComponent<Rigidbody>().velocity=v;

		networkScript.sendAttack (1);
	}

	private void fatalFoudre()
	{
		anim.Play("Armature_Hit", -1, 0f);
		Vector3 position = transform.position;
		position = new Vector3 (position.x, position.y+10, position.z);
		GameObject eclair = Instantiate (FatalFoudre, position, Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(GetComponent<Collider>(), eclair.GetComponent<Collider>());
		eclair.GetComponent<Rigidbody>().velocity=new Vector3 (0, -15, 0);
		
		networkScript.sendAttack (2);
	}




	private void attack()
	{
		if(Time.time-lastAttack>1)
		{
			if (Input.GetKeyDown (KeyCode.Alpha1) && !Input.GetKey (KeyCode.DownArrow)) {
					eclair ();
					lastAttack=Time.time;
			} else if (Input.GetKeyDown (KeyCode.Alpha1)) {
					fatalFoudre ();
					lastAttack=Time.time;
			}
		}
	}





	private void move()
	{
		float horizontal = Input.GetAxis ("Horizontal");
		bool jump = jumpLeft>0 && Input.GetButtonDown ("Jump");
		if (jump) {
			anim.Play ("Armature_Jump", -1, 0f);
			jumpLeft--;
			jumping=true;
			Vector3 v = new Vector3(GetComponent<Rigidbody>().velocity.x, 5, 0);
			GetComponent<Rigidbody>().velocity=v;
		}
		
		Vector2 mouvmentH = new Vector2(horizontal, 0);
		//transform.Translate (mouvmentH * 5 * Time.deltaTime);
		GetComponent<Rigidbody>().AddForce (mouvmentH*Time.deltaTime*800);
		if (Mathf.Abs (GetComponent<Rigidbody>().velocity.x) > 3) 
		{
			var newMouvment = GetComponent<Rigidbody>().velocity;
			newMouvment.x = 3*Mathf.Sign(GetComponent<Rigidbody>().velocity.x);
			GetComponent<Rigidbody>().velocity = newMouvment;
		}
		
		if (Mathf.Abs (horizontal) <= 0.01f) {
			running=false;
			if(!jumping)
			{
				var newMouvment = GetComponent<Rigidbody>().velocity;
				newMouvment.x = 0.7f * newMouvment.x;
				GetComponent<Rigidbody>().velocity = newMouvment;
			}
		} else {
			running=true;
			if(horizontal>0)
			{
				transform.rotation=Quaternion.AngleAxis(-90, Vector3.up);
			}
			else
			{
				transform.rotation=Quaternion.AngleAxis(90, Vector3.up);
			}
		}
	}
}