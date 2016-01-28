using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisMapGenerator : MonoBehaviour {

	enum TILES { deep_water, shallow_water, sand, grass, soft_montain, hard_montain}

	public int      _width = 64;
	public int      _height = 64;
	public float    _mapScale = 0.25f;
	public float    _timeScale = 0.15f;
	public Sprite[] _tileSet;

	// Contains the tile values
	private int [,] _tileMap;

	private GameObject[,] _tiles;

	private readonly int tileSize = 32 - 1;

	// Use this for initialization
	void Start () {

		_tileMap = new int[_width, _height];

		InitTerrain ();

		GenerateTerrain (Time.time);
	}

	void InitTerrain() {

		_tiles = new GameObject[_width,_height];

		GameObject terrain = new GameObject ();
		terrain.name = "Terrain";

		for (int i = 0; i < _width; i++) {

			for (int j = 0; j < _height; j++) {

				_tiles [i, j] = CreateTile (i, j, terrain.transform);
			}
		}
	}

	GameObject CreateTile(int i, int j, Transform terrain) {

		float worldTileSize = (float)tileSize/ (float)MisConstants.PIXEL_UNIT;

		// Creates a game object for the tile
		GameObject gameObj = new GameObject ();
		gameObj.name = "Tile_" + i + "_" + j;

		// Set position and parent
		gameObj.transform.parent = terrain;
		gameObj.transform.position = new Vector2 (i, j) * worldTileSize;

		// Add sprite renderer to support the texture
		gameObj.AddComponent<SpriteRenderer> ();

		return gameObj;
	}

	void GenerateTerrain(float time) {

		PerlinNoise noiseGen = new PerlinNoise (0);

		for (int i = 0; i < _width; i++) {

			for (int j = 0; j < _height; j++) {

				float x = (float)i / (float)_width;
				float y = (float)j / (float)_height;

				float noise = noiseGen.FractalNoise3D(x, y, time, 2, _mapScale + 0.01f, 1.75f);

				_tileMap[i,j] = NoiseToTile (noise);

				RenderTile (_tiles[i,j], _tileMap[i,j]);
			}
		}
	}

	void RenderTile(GameObject tile, int tileValue) {

		SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer> ();
		tileRenderer.sprite = _tileSet[tileValue];
	}

	int NoiseToTile(float noiseValue) {

		noiseValue = Mathf.Clamp (noiseValue, -2f, 2f);

		if (noiseValue >= -2f  && noiseValue < 0f)
			return (int)TILES.deep_water;

		if (noiseValue >= 0f   && noiseValue < 0.25f)
			return (int)TILES.shallow_water;

		if (noiseValue >= 0.25f && noiseValue < 0.4f)
			return (int)TILES.sand;

		if (noiseValue >= 0.4f  && noiseValue < 0.75f)
			return (int)TILES.grass;

		if (noiseValue >= 0.75f && noiseValue < 0.95f)
			return (int)TILES.soft_montain;

		return (int)TILES.hard_montain;
	}
}
