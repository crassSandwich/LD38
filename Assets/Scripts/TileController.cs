﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

	public static GameControl MainControlScript;
	public TileController m_northTile;
	public TileController m_eastTile;
	public TileController m_southTile;
	public TileController m_westTile;
	public bool m_playerHere;
	public Vector2 position;
	public Animator playerAnim;

	private SpriteRenderer spriteR;
	private GameObject letterPart;
	private GameObject playerPart;
	private GameObject enemyPart;
	private int letter;
	private string letterS;

	public void Initialize (Vector2 position, int n) {
		MainControlScript = GameObject.Find ("GameController").GetComponent<GameControl> ();

		transform.position = new Vector3 (position.x, position.y);
		this.position = position;
		gameObject.name = "Tile" + position.ToString ();

		letterPart = transform.Find ("Letter").gameObject;
		playerPart = transform.Find ("PlayerPart").gameObject;
		enemyPart = transform.Find ("Enemy").gameObject;
		spriteR = gameObject.GetComponent<SpriteRenderer> ();
		playerAnim = playerPart.GetComponent<Animator> ();

		letter = MainControlScript.AssignLetter (position);
		letterPart.GetComponent<SpriteRenderer> ().sprite = MainControlScript.LetterSprites [letter];
		letterS = ((char)(letter + "a"[0])).ToString();
		Debug.Log (letterS);
			
		m_northTile = null;
     	m_eastTile = null;
		m_southTile = null;
		m_westTile = null;
		CheckSurroundings ();

		if (n > 0)
			MakeBranches (n);
	}
	
	// Update is called once per frame
	void Update () {
		PlayerControl ();
		CheckSurroundings ();
	}


	//check if there are any new neighboring tiles, and update this one appropriately 
	void CheckSurroundings () {
		bool change = false;
		if (m_northTile == null) {
			m_northTile = MainControlScript.GetTileAtPosition (new Vector2 (position.x, position.y + 1));
			if (m_northTile != null)
				change = true;
		}
		if (m_eastTile == null) {
			m_eastTile = MainControlScript.GetTileAtPosition (new Vector2 (position.x + 1, position.y));
			if (m_eastTile != null)
				change = true;
		}
		if (m_southTile == null) {
			m_southTile = MainControlScript.GetTileAtPosition (new Vector2 (position.x, position.y - 1));
			if (m_southTile != null)
				change = true;
		}
		if (m_westTile == null) {
			m_westTile = MainControlScript.GetTileAtPosition (new Vector2 (position.x - 1, position.y));
			if (m_westTile != null)
				change = true;
		}
		if (change) {
			spriteR.sprite = MainControlScript.GetTileSpriteByDirection (m_northTile != null, m_eastTile != null, m_southTile != null, m_westTile != null);
		}
			
	}

	//generate branching structure to level
	void MakeBranches (int n) {
		//Debug.Log ("here");
		//first, make a list of the position of every open neighboring tile
		CheckSurroundings (); //probably unnecessary, but calling this to be 
		List<Vector2> neighbors = new List<Vector2> ();
		if (m_northTile == null)
			neighbors.Add (new Vector2(position.x, position.y+1));
		if (m_eastTile == null)
			neighbors.Add (new Vector2(position.x+1, position.y));
		if (m_southTile == null)
			neighbors.Add (new Vector2(position.x, position.y-1));
		if (m_westTile == null)
			neighbors.Add (new Vector2(position.x-1, position.y));
		
		//generate n/2 neighboring tiles
		//first, reduce the list to at most n/2 spots
		while (neighbors.Count > n/2) {
			int discard = Random.Range (0, neighbors.Count);
			neighbors.RemoveAt (discard);
		}
		neighbors = Shuffle (neighbors);
		//then make a new tile for each spot that's still open
		foreach (Vector2 pos in neighbors) {
			//first check to see if one of your child branches already took this spot
			if (MainControlScript.GetTileAtPosition(pos) == null)
				MainControlScript.CreateTile (pos, n - 1);
		}

	}

	//fisher-yates shuffle a list of Vector2s. one-off utility function for MakeBranches
	List<Vector2> Shuffle(List<Vector2> unshuffled) {
		int j;
		Vector2 temp;
		List<Vector2> a = new List<Vector2> (unshuffled);
		for (int i = a.Count - 1; i > 1; i--) {
			j = Random.Range (0, i);
			temp = a [j];
			a [j] = a [i];
			a [i] = temp;
		}
		return a;
	}

	void PlayerControl () {
		if (MainControlScript.IsDamaged())
			return;
		if (Input.GetKeyDown (letterS)) {
			//when the player initially occupies a tile
			//playerAnim.SetBool ("exists", true);
			MainControlScript.AddToPlayer (position);
		} else if (Input.GetKey(letterS)) {
			//while the player holds down on a tile
		} else if (Input.GetKeyUp(letterS)) {
			//when the player releases from a tile
			playerAnim.SetBool ("exists", false);
			MainControlScript.Damage ();
		}
	}

	
}
