using UnityEngine;
using System.Collections;

public class MenuPause : MonoBehaviour {
	#region Attributs
	
	private bool isPaused = false; 
	
	#endregion
	
	#region Proprietes
	#endregion
	
	#region Constructeur
	#endregion
	
	#region Methodes



	void Start () {
	
	}
	

	void Update () {

		if(Input.GetKeyDown(KeyCode.Escape))
			isPaused = !isPaused;
		
		
		if(isPaused)
			Time.timeScale = 0f; 
		
		else
			Time.timeScale = 1.0f;
		
		

	
	}
	
	void OnGUI ()
	{
		if(isPaused)
		{
			

			if(GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 20, 80, 40), "Continuer"))
			{
				isPaused = false;
			}
			if(GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 70, 80, 40), "Quitter"))
			{
				Application.Quit(); 
			}

			

			if(GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 + 40, 80, 40), "Menu"))
			{

				Application.LoadLevel("Menu Principal"); 
				
			}
			
		}
	}
	
	#endregion

}
