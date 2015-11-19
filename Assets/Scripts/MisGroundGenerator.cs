﻿using UnityEngine;
using System.Collections;

public class MisGroundGenerator : MonoBehaviour {

	public Sprite  []_surface;
	public Sprite  []_intertal;
	public Sprite  []_external;
	public Sprite  []_platforms;

	public Vector2 _startPosition;
	public Vector2 _endPosition;
	public Vector2 _colliderOffset;

	private Transform _lastSurface;

	// Use this for initialization
	void Start () {

		if (_surface.Length == 0) {

			Debug.Log("Surface array must have at least one element.");
			return;
		}

		if (_intertal.Length == 0) {
			
			Debug.Log("Intertal array must have at least one element.");
			return;
		}
	}

	public void GenerateLevel() {

		GenerateGround (_startPosition.x, _endPosition.x, 
		                _startPosition.y, MisConstants.TILE_SIZE/100f);
	}

	private void GenerateGround(float startPosX, float endPosX, float startPosY, float tileSize) {

		if (startPosX > endPosX)
			return;

		GameObject Ground = new GameObject();
		Ground.name = "Ground";

		int gapsSequence = MisConstants.MAX_GAP_SIZE;
		for (float i = startPosX; i < endPosX; i += tileSize) {

			GameObject surfaceI = new GameObject();
			surfaceI.name = "Surface";
			surfaceI.transform.parent = Ground.transform;
			surfaceI.transform.position = new Vector2(i, startPosY);	

			// Randomly decides if it will be a whole
			if(gapsSequence < MisConstants.MAX_GAP_SIZE) {

				if(Random.value < 0.15f) {

					FullfillWall(surfaceI.transform, 3f, tileSize, true);

					// Create a kill zone below the whole
					Vector2 killOffset = new Vector2(0f, -tileSize*3f);
					Vector2 zoneSize   = new Vector2(tileSize + 1f, tileSize);
					Vector2 respawnPos = _lastSurface.position + new Vector3(0f, tileSize, 0f);
					CreateKillZone(surfaceI, killOffset, zoneSize, respawnPos);

					gapsSequence++;
					continue;
				}

				gapsSequence = MisConstants.MAX_GAP_SIZE;
			}

			gapsSequence = 0;

			// Add surface sprite to the new surface object
			SpriteRenderer renderer = surfaceI.AddComponent<SpriteRenderer>();
			renderer.sprite = _surface[Random.Range(0, _surface.Length)];

			// Add box colliders to the new surface objects
			BoxCollider2D bCollider = surfaceI.AddComponent<BoxCollider2D>();
			bCollider.offset = _colliderOffset;

			//FulfillSurface(surfaceI.transform, 1f, tileSize);
			FullfillWall(surfaceI.transform, 3f, tileSize);

			// Randomly add platform above this surface
			if(Random.value < 0.15f)
				AddPlatform(surfaceI.transform, 1f,tileSize);

			// Randomly changes the height of the ground
			if(Random.value < 0.15f) {

				float coin = (Random.value < 0.5f ? 1f : -1f);
				startPosY += (MisConstants.TILE_SIZE/100f) * coin;
			}

			_lastSurface = surfaceI.transform;
		}
	} 

	// Fullfill area below the surface with background tile
	private void FulfillSurface(Transform surface, float distBelow, float tileSize) {

		if (distBelow < 0)
			return;

		float surfacePosX = surface.position.x;
		float surfacePosY = surface.position.y;

		for(float i = surfacePosY; i > surfacePosY - distBelow; i -= tileSize) {

			GameObject internalI = new GameObject();
			internalI.name = "Interal";
			internalI.transform.parent = surface;
			internalI.transform.position = new Vector2(surfacePosX, i);

			// Add surface sprite to the new object
			SpriteRenderer renderer = internalI.AddComponent<SpriteRenderer>();
			renderer.sprite = _intertal[Random.Range(0, _intertal.Length)];
		}
	}

	// Fullfill area above the surface with background tile
	private void FullfillWall(Transform surface, float distAbove, float tileSize, bool isWhole = false) {

		if (distAbove < 0)
			return;
		
		float surfacePosX = surface.position.x;
		float surfacePosY = surface.position.y;

		float tilePosY = surface.position.y;
		if (isWhole)
			tilePosY -= tileSize;

		for(float i = tilePosY + tileSize; i < surfacePosY + distAbove; i += tileSize) {
			
			GameObject internalI = new GameObject();
			internalI.name = "Interal";
			internalI.transform.parent = surface;
			internalI.transform.position = new Vector2(surfacePosX, i);
			
			// Add surface sprite to the new object
			SpriteRenderer renderer = internalI.AddComponent<SpriteRenderer>();
			renderer.sprite = _external[Random.Range(0, _external.Length)];
		}
	}

	private void AddPlatform(Transform surface, float distAbove, float tileSize) {

		if (distAbove < 0 || _platforms.Length == 0)
			return;

		float surfacePosX = surface.position.x;
		float surfacePosY = surface.position.y;

		float platformPosY = surfacePosY + distAbove * tileSize;

		GameObject platformI = new GameObject();
		platformI.name = "Platform";
		platformI.transform.parent = surface;
		platformI.transform.position = new Vector2(surfacePosX, platformPosY);

		// Add surface sprite to the new object
		SpriteRenderer renderer = platformI.AddComponent<SpriteRenderer>();
		renderer.sprite = _platforms[Random.Range(0, _platforms.Length)];

		BoxCollider2D bCollider = platformI.AddComponent<BoxCollider2D>();
		bCollider.offset = _colliderOffset;
	}

	private void CreateKillZone(GameObject surface, Vector2 offset, Vector2 size, Vector2 respawnPos) {

		GameObject whole = new GameObject ();
		whole.transform.parent = surface.transform;
		whole.transform.position = surface.transform.position;

		MisKillZone killZone = whole.AddComponent<MisKillZone> ();
		killZone.respawnPosition = respawnPos;

		BoxCollider2D collider = whole.AddComponent<BoxCollider2D> ();
	
		collider.isTrigger = true;
		collider.offset = offset;
		collider.size = size;

		whole.tag = MisConstants.TAG_KILLZONE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
