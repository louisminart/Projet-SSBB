using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Xml;


//objet qui gère la communiquation avec le réseau
//pour le faire marcher, lancer le projet visual studio sur le meme ordi
//pour l'instant ça marche que en local






public class Network : MonoBehaviour {
	
	
	
	private TcpClient tcpclnt;				//contient la connexion au serveur (ip, tout ça...)
	public GameObject gob;					//le "modèle" à utiliser pour les autres joueurs, à mettre depuis Unity
	public GameObject player;				
	public GameObject Eclair;
	public GameObject FatalFoudre;
	private int id;							//entier servant à différencier les joueurs entre eux, envoyé par le serveur à la connexion
	private List<Player> otherPlayers;		//liste des autres joueurs (Player : gameobject + id liée au joueur)

	private string adresse="127.0.0.1";
	private int port=81;
	
	
	
	
	//structures qui sont échangées entre les joueurs
	//(structure : un peu comme un objet sans fonction et avec tout en public)
	
	
	//nouveau joueur
	public struct NewPlayer
	{
		public int id;	//meme chose que l'id au dessus, pour différencier les joueurs
		public float x;	//position (pour l'instant ça marche pas)
		public float y;	
	}
	
	//déplacement d'un joueur
	public struct MovePlayer
	{
		public int id;
		public float x;
		public float y;
		public float moveX;
		public float moveY;
	}
	
	
	//attaque
	public struct Attack
	{
		public int id;
		public int type;
	}
	
	
	
	
	
	
	//sert à faire le lien entre un joueur et son id (c'est sale mais j'ai pas trouvé mieux)
	//pas échangé entre les joueurs
	public struct Player
	{
		public GameObject gob;
		public int id;
	}
	
	
	
	
	public void readData(byte[] data)
	{
		if (data [0] != 0) {
						int type = data [0];	//le premier octet représente le type de structure
						Debug.Log (type);
						switch (type) {
						case 1:
								receiveNewPlayer (data);
								break;
			
						case 2:
								receiveMovePlayer (data);
								break;
			
						case 3:
								receiveAttack (data);
								break;

						case 4:
								receivePlayerLeft (data);
								break;
						}
				}
	}
	
	
	
	
	private void receiveNewPlayer(byte[] data)
	{
		try
		{
			//crée et lit la structure
			NewPlayer np;
			np = (NewPlayer) ByteArrayToClass(data, typeof(NewPlayer));
			//np contient donc l'id et la position d'un nouveau joueur
			
			if (np.id != id) {	//on va forcément se recevoir soi-meme au début, faut pas ajouter un joueur dans ce cas
				
				//sinon on crée un nouveau joueur avec les données reçues
				Vector3 v = new Vector3(np.x, np.y, 0);
				GameObject player = Instantiate (gob, v, Quaternion.identity) as GameObject;
				
				Physics.IgnoreCollision(player.GetComponent<Collider>(), this.player.GetComponent<Collider>());	//cette ligne sert à ignorer les collisions entre les joueurs
				foreach(Player other in otherPlayers)						//pareil pour tous les autres
				{
					Physics.IgnoreCollision(player.GetComponent<Collider>(), other.gob.GetComponent<Collider>());
				}
				Player p;
				p.gob=player;
				p.id=np.id;
				otherPlayers.Add (p);
			}
		}
		catch(Exception e)
		{
			Debug.Log (e.Message+e.StackTrace);
		}
	}
	
	
	
	//crée une structure MovePlayer à partir du mouvement effectué, et l'envoie
	public void sendMove()
	{
		MovePlayer self;
		self.x = player.transform.position.x;
		self.y = player.transform.position.y;
		self.moveX = player.GetComponent<Rigidbody>().velocity.x;
		self.moveY = player.GetComponent<Rigidbody>().velocity.y;
		self.id = id;
		byte[] b = ClassToByteArray (self, typeof(MovePlayer));
		sendData (b, 2);
	}
	
	//meme principe que receiveNewPlayer
	//changer uniquement la position provoquerait un mouvement saccadé comme sur les autres projets
	//donc on change la position ET la direction, pour anticiper les mouvements
	private void receiveMovePlayer(byte[] data)
	{
		MovePlayer mp;
		try{
			mp = (MovePlayer) ByteArrayToClass(data, typeof(MovePlayer));
			
			if (mp.id != id) {
				Vector3 position = new Vector3(mp.x, mp.y, 0);
				Vector3 move = new Vector3(mp.moveX, mp.moveY, 0);
				foreach(Player p in otherPlayers)
				{
					if(mp.id==p.id)
					{
						p.gob.GetComponent<Rigidbody>().MovePosition(position);
						p.gob.GetComponent<Rigidbody>().velocity=move;
					}
				}
			}
		}
		catch(Exception e)
		{
		}
	}
	
	
	
	public void sendAttack(int type)
	{
		Attack att;
		att.id = id;
		att.type = type;
		byte[] b = ClassToByteArray (att, typeof(Attack));
		sendData (b, 3);
	}
	
	private void receiveAttack(byte[] data)
	{
		Attack att = (Attack) ByteArrayToClass(data, typeof(Attack));
		foreach (Player p in otherPlayers) 
		{
			if(p.id==att.id)
			{
				switch(att.type)
				{
				case 1 :
					receiveEclair(p.gob);
					break;

				case 2 :
					receiveFatalFoudre(p.gob);
					break;
				}
				p.gob.GetComponent<Animation>().animAttack();
			}
		}
	}
	private void receiveEclair(GameObject p)
	{
		Vector3 position = p.transform.position;
		position = new Vector3 (position.x, position.y+0.5f, position.z);
		GameObject eclair = Instantiate (Eclair, position, Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(p.GetComponent<Collider>(), eclair.GetComponent<Collider>());
		float x = p.transform.rotation.y < 0 ? 10 : -10;
		Vector3 v = new Vector3 (x, 0, 0);
		eclair.GetComponent<Rigidbody>().velocity=v;
	}

	private void receiveFatalFoudre(GameObject p)
	{
		Vector3 position = p.transform.position;
		position = new Vector3 (position.x, position.y+10, position.z);
		GameObject eclair = Instantiate (FatalFoudre, position, Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(p.GetComponent<Collider>(), eclair.GetComponent<Collider>());
		eclair.GetComponent<Rigidbody>().velocity=new Vector3 (0, -10, 0);
	}
	
	private void receivePlayerLeft(byte[] data)
	{
		try
		{
			NewPlayer np;
			np = (NewPlayer) ByteArrayToClass(data, typeof(NewPlayer));
			
			if (np.id != id) {
				for(int i=otherPlayers.Count-1; i>=0; i--)
				{
					if(np.id==otherPlayers[i].id)
					{
						Destroy(otherPlayers[i].gob);
						otherPlayers.RemoveAt(i);
					}
				}
			}
		}
		catch(Exception e)
		{
			Debug.Log (e.Message+e.StackTrace);
		}
	}
	
	
	
	
	
	
	
	
	
	
	//en dessous c'est des fonctions qui seront appelées depuis l'objet lui meme, pour simplifier le reste du code
	
	
	
	
	
	
	
	
	
	
	
	//initialise tout ce bordel et envoie un objet NewPlayer au serveur, qui sera renvoyé à tout le monde
	void Start () {
		tcpclnt = new TcpClient();
		otherPlayers = new List<Player> ();
		id=0;
		try
		{
			StreamReader sr = new StreamReader("IP.txt", Encoding.Default);
			adresse=sr.ReadLine();
			Debug.Log(adresse);
		}
		catch(Exception e){}
		try
		{
			//initialisation
			Debug.Log("Connecting.....");
			tcpclnt.Connect(adresse, port);
			Debug.Log("Connected");
			
			NetworkStream stream = tcpclnt.GetStream();
			Byte[] data = new Byte[4];
			stream.Read(data, 0, data.Length);
			id=BitConverter.ToInt32(data, 0);
			Debug.Log("id : "+id);
			
			
			//envoi 
			NewPlayer self;
			self.x = player.transform.position.x;
			self.y = player.transform.position.y;
			self.id=id;
			byte[] b = new byte[1024] ;
			b=ClassToByteArray(self, typeof(NewPlayer));
			sendData(b, 1);
			receiveData();
		}
		catch (SocketException e)
		{
			Debug.Log("Error..... " + e.ErrorCode + e.StackTrace);
		}
	}
	
	
	//récéption des données à chaque frame
	void Update () {
		receiveData ();
	}
	
	
	
	
	
	
	//fonction executee a chaque boucle, verifie si il y a des donnees en attente
	public void receiveData()
	{
		while (tcpclnt.Available > 0)
			//tant qu'il y a des trucs à recevoir
		{
			try
			{
				NetworkStream stream = tcpclnt.GetStream();		//trucs qui servent juste à "initialiser" la récéption des données
				Byte[] data = new byte[1024];					//pas bien important
				Array.Clear(data, 0, data.Length);				//
				stream.Read(data, 0, data.Length);				//
				
				
				
				//là ça devient tordu, ça "sépare" les structures envoyées si il y en a plusieurs à la suite
				//parce que si on fait deux envois trop rapides, ils seront "fusionnés" sans prévenir
				//pour les différencier, un 0 sépare les structures dans le tableau
				
				int i=0;	//position dans le tableau de byte reçu
				while(i<data.Length)	
				{
					while(i<data.Length&&data[i]==0)	//on se décale jusqu'à tomber sur autre chose que des 0
					{									//
						i++;							//
					}									//
					
					
					byte[] b = new byte[1024];	//on crée un autre tableau, pour "séparer" une structure
					int j=0;	//position dans le 2e tableau
					
					while(i<data.Length&&data[i]!=0)	//tant qu'on a pas atteint le bout ou un 0
					{									//on copie tout
						b[j]=data[i];					//
						j++;							//
						i++;							//
					}									//
					
					
					readData(b);	//et on appelle la fonction qui va trier tout ça
				}
				
				
			}
			catch (Exception e)
			{
				Debug.Log("Error..... " + e.GetType() + e.StackTrace);
			}
		}
	}
	
	
	
	//envoie une chaine de bytes au serveur
	//le second argument correspond au type de structure (1:NewPlayer, 2:MovePlayer...)
	//il sera ajouté en tete du tableau
	public void sendData(byte[] data, byte type)
	{
		try
		{
			NetworkStream stream = tcpclnt.GetStream();
			byte[] d = addToBegin(data, type);
			stream.Write(d, 0, d.Length);
		}
		catch (Exception e)
		{
			Debug.Log("Error..... " + e.StackTrace);
		}
	}
	
	
	
	
	
	//serialize = transforme l'objet en tableau de byte
	//en format XML mais on s'en fout
	private byte[] ClassToByteArray(object objClass, Type type)
	{
		try
		{
			MemoryStream ms = new MemoryStream();
			XmlSerializer xmlS = new XmlSerializer(type);
			XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);
			
			xmlS.Serialize(xmlTW, objClass);
			ms = (MemoryStream)xmlTW.BaseStream;
			
			return ms.ToArray();
		}
		catch (Exception)
		{
			throw;
		}
	}
	
	//pareil dans l'autre sens
	private object ByteArrayToClass(byte[] buffer, Type type)
	{
		try
		{
			XmlSerializer xmlS = new XmlSerializer(type);
			MemoryStream ms = new MemoryStream(buffer, 1, buffer.Length-1);
			XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);
			
			return xmlS.Deserialize(ms);
		}
		catch (Exception e)
		{
			throw;
		}
	}
	
	//ajoute un truc en tete d'un tableau
	//très sale mais bon...
	private byte[] addToBegin(byte[] b, byte x)
	{
		byte[] r = new byte[b.Length + 2];
		r [0] = x;
		for (int i=0; i<b.Length; i++) {
			r [i + 1] = b [i];
		}
		r [r.Length - 1] = 0;
		return r;
	}
}
