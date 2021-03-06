﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SynchronizerData;

public class MetronomeLevelGenerator : MisLevelGenerator {

	private float []_spectrum;

	private int _lastAddedPlatform;
	private float _lastAnglePlatform;

	private Vector2    _currentGroundPos;
	private Vector2    _currentPlarfoPos;

	private BeatCounter      _beatCounter;
	private BeatObserver     _beatObserver;
	private BeatSynchronizer _beatSync;

	private PerlinNoise  _noiseGenerator;

	private MisObjectPool _tilesSource;
	private MisObjectPool _enemiesSource;

	private List<GameObject> _tilesAdded;
	private List<GameObject> _tilesToDelete;
	protected Dictionary <Vector2, GameObject> _collidebleTiles;

	private MisBoss _boss;
	public MisBoss Boss { get { return _boss; } }

	public static readonly int FST_SECTION = 8;
	public static readonly int SND_SECTION = 16;
	public static readonly int SND_SECTION_HEIGHT = 4;
	public static readonly int FST_JMP_SIZE = 4;
	public static readonly int PLAT1_POS = 3;
	public static readonly int PLAT2_POS = 12;
	public static readonly int SONG_SAMPLE_SIZE = 256;

	protected override void Start() {

		base.Start ();

		_beatSync = GetComponent<BeatSynchronizer>();
		_beatCounter = GetComponent<BeatCounter>();
		_beatObserver = GetComponent<BeatObserver>();

		_beatSync.enabled = false;
		_boss.gameObject.SetActive(false);

		_tilesSource  = new MisObjectPool (_platforms[(int)PLATFORMS.BREAKABLE], 300, transform);
		_tilesSource.ExecActionInObjects (InitTile);

		_enemiesSource  = new MisObjectPool (_enemies[(int)ENEMIES.MINION], 50, transform);
		_enemiesSource.ExecActionInObjects (InitEnemy);

		_tilesAdded    = new List<GameObject> ();
		_tilesToDelete = new List<GameObject> ();
		_collidebleTiles = new Dictionary <Vector2, GameObject> ();

		_noiseGenerator = new PerlinNoise (0);

		_lenght = (int)(_beatCounter.audioSource.clip.length * 8f);
		_lenght += FST_SECTION + SND_SECTION + FST_JMP_SIZE;

		_spectrum = new float[SONG_SAMPLE_SIZE];
	}

	void Update() {
		
		_boss.FollowingPoint.x = _currentGroundPos.x;
		_boss.FollowingPoint.y = _currentPlarfoPos.y;

		if ((_beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat) {

			if ((int)(_currentGroundPos.x / MisConstants.TILE_SIZE) < _lenght) {

				AddPlatform ();
				PlaceGround ();
			}
		}

		if ((_beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat) {

			if ((int)(_currentGroundPos.x / MisConstants.TILE_SIZE) < _lenght) {

				AddEnemy (_currentPlarfoPos);
//				AddEnemy (_currentGroundPos);
			}
		}	

		MisHero hero = MisGameWorld.Instance.WorldHero;

		if(hero) {

			float cameraWidth = MisGameWorld.Instance.WorldCamera.GetCameraWidth ();

			foreach (GameObject platform in _tilesAdded) {

				if (_currentGroundPos.x > platform.transform.position.x && 
					_currentGroundPos.x - platform.transform.position.x > cameraWidth) {

					_tilesSource.SetFreeObject (platform);
					_tilesToDelete.Add (platform);
				}
			}
				
			foreach (GameObject platform in _tilesToDelete)
				_tilesAdded.Remove (platform);

			_tilesToDelete.Clear ();
		}
	}

	private int[,] AddSpawnPointsToPlatform(int type, float rotInDegrees) {

		int[,] platformDescriptor = MisConstants.PLATFORM_DESCRIPTOR[(int)type];

		int timesToRotate = (int)rotInDegrees / 90;

		for (int i = 0; i < timesToRotate; i++)
			platformDescriptor = MisMath.Rotate2DArrayClockwise (platformDescriptor);

		return platformDescriptor;
	}
		
	protected override void GenerateLevel(Vector2 startPos, int levelLenght) {

		_currentGroundPos = startPos + Vector2.up * MisConstants.LEVEL_GROUND_HEIGHT * MisConstants.TILE_SIZE;

		for (int i = 0; i < FST_SECTION; i++) {

			GameObject newPlat = (GameObject)Instantiate (_platforms [(int)PLATFORMS.STATIC1], 
				_currentGroundPos,Quaternion.identity);
			newPlat.transform.parent = _level.transform;

			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT);
			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
		}
			
		_currentGroundPos.x += MisConstants.TILE_SIZE * FST_JMP_SIZE;
		_currentGroundPos.y += MisConstants.TILE_SIZE * SND_SECTION_HEIGHT;

		for (int i = 0; i < SND_SECTION; i++) {

			if (i == PLAT1_POS) {

				GameObject firstPlat1 = (GameObject) Instantiate (_platforms [(int)PLATFORMS.TETRIS1], 
					_currentGroundPos + Vector2.up * 1f * MisConstants.TILE_SIZE, Quaternion.identity);
				firstPlat1.transform.parent = _level.transform;

				GameObject firstPlat2 = (GameObject) Instantiate (_platforms [(int)PLATFORMS.TETRIS1], 
					_currentGroundPos + Vector2.up * 4f * MisConstants.TILE_SIZE, Quaternion.identity);
				firstPlat2.transform.parent = _level.transform;

				GameObject firstEnemy = (GameObject) Instantiate (_enemies [(int)ENEMIES.MINION], 
					_currentGroundPos + Vector2.one * 1f * MisConstants.TILE_SIZE, Quaternion.identity);
				firstEnemy.transform.parent = _level.transform;
			}

			if (i == PLAT2_POS) {

				GameObject metroPlat = (GameObject) Instantiate (_platforms [(int)PLATFORMS.EVENT],
					_currentGroundPos + Vector2.up * MisConstants.TILE_SIZE, Quaternion.identity);
				metroPlat.transform.parent = _level.transform;

				CreateEventPlatform (metroPlat, StartMetronome);
			}

			GameObject newPlat = (GameObject) Instantiate (_platforms [(int)PLATFORMS.STATIC1],
				_currentGroundPos, Quaternion.identity);
			newPlat.transform.parent = _level.transform;

			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT + SND_SECTION_HEIGHT);

			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
		}

		// Spawn the boss
		Vector3 bossPos = _currentGroundPos;
		bossPos.z = _enemies [(int)ENEMIES.BOSS].transform.position.z;

		GameObject objBoss = (GameObject)Instantiate (_enemies[(int)ENEMIES.BOSS], bossPos, Quaternion.identity);
		objBoss.transform.parent = _level.transform;
		_boss = objBoss.GetComponent<MisBoss> ();

		_currentGroundPos.y -= MisConstants.TILE_SIZE * (SND_SECTION_HEIGHT + 2f);
	} 

	// Fullfill area below the surface with background tile
	private void FulfillGround(GameObject groundPlat, int height) {

		Vector2 startPos = groundPlat.transform.position;

		for(int j = 0; j < height; j++) {

			Vector2 pos = new Vector2 (startPos.x, startPos.y - (j + 1) *  MisConstants.TILE_SIZE);

			GameObject newPlat = (GameObject) Instantiate (_platforms [(int)PLATFORMS.STATIC2],pos,Quaternion.identity);
			newPlat.transform.parent = _level.transform;
		}
	}

	private void CreateEventPlatform(GameObject surface, MisEventPlatform.PlatformEvent platEvent, int eventLimit = 1) {

		MisEventPlatform killZone = surface.AddComponent<MisEventPlatform> ();

		killZone.EventLimit = eventLimit;
		killZone.Normal = new Vector2 (-1f, 0f);
		killZone.Event = platEvent;

		surface.tag = PLATFORMS.EVENT.ToString();
	}
		
	private void AddPlatform() {

		_beatCounter.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

		float x =  MisMath.Mean(_spectrum) * 20f;
		float noiseTile = _noiseGenerator.FractalNoise1D (x, 5, _lenght * 0.00001f, 1.8f);
		float noisePos  = _noiseGenerator.FractalNoise1D (x, 5, _lenght * 0.00001f, 1.8f);
		float noiseRot  = _noiseGenerator.FractalNoise1D (x, 5, _lenght * 0.00001f, 1.8f);

		Vector2 platPos = new Vector2(_currentGroundPos.x, NoiseToTileHeight(noisePos) * MisConstants.TILE_SIZE);

		int tileType = NoiseToTile (noiseTile);
		float tileRot = NoiseToRotation (noiseRot);
		BuildTile (tileType, platPos, Vector3.forward * tileRot);

		_lastAddedPlatform = tileType;
		_lastAnglePlatform = tileRot;
		_currentPlarfoPos = platPos;
	}

	private void AddEnemy(Vector3 referencePos) {

		int[,] spawnPoints = MisConstants.PLATFORM_DESCRIPTOR [_lastAddedPlatform];

		int xPos = 0;
		int yPos = 0;

		for (int i = 0; i < spawnPoints.GetLength (0); i++) {

			for (int j = 0; j < spawnPoints.GetLength (1); j++) {

				if (spawnPoints [i, j] == 1) {

					if (j + 1 < spawnPoints.GetLength (1) && spawnPoints [i, j + 1] == 0) {

						xPos = i;
						yPos = j + 1;
						break;
					} 
					else if (j + 1 >= spawnPoints.GetLength (1)) {

						xPos = i;
						yPos = j + 1;
						break;
					}
				}
			}
		}

		Vector3 platPos = referencePos + new Vector3(xPos - 0f, yPos) * MisConstants.TILE_SIZE;
		platPos.z = _enemies [(int)ENEMIES.MINION].transform.position.z;

		GameObject enemy = _enemiesSource.GetFreeObject();
		enemy.transform.parent = _level.transform;
		enemy.transform.position = platPos;

		enemy.GetComponent<MisEnemy> ().Velocity = Vector2.zero;
	}

	public void StartMetronome() {

		_beatSync.enabled = true;
		_boss.gameObject.SetActive(true);
		MisHUD.Instance.bossHealthBar.gameObject.SetActive (true);

		MisHUD.Instance.timer.Pause = false;
	}

	public void StopMetronome() {

		_beatSync.enabled = false;
		_beatCounter.audioSource.Stop ();
		MisHUD.Instance.bossHealthBar.gameObject.SetActive (false);
		MisHUD.Instance.timer.Pause = true;
	}

	public void DestroyMetronome() {

		_beatSync.gameObject.SetActive (false);

		MisHUD.Instance.bossHealthBar.gameObject.SetActive (false);
		MisHUD.Instance.timer.Pause = true;
	}

	private void PlaceGround() {

		_beatCounter.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

		float x =  MisMath.Mean(_spectrum) * 20f;
		float noisePos = _noiseGenerator.FractalNoise1D (x, 5, _lenght * 0.00001f, 1.8f);

		int height = NoiseToGroundHeight (noisePos);

		if (height > 0) {
			
			BuildTile ((int)PLATFORMS.TETRIS1, _currentGroundPos, Vector3.zero);
			_currentGroundPos.y = (height - 2) * MisConstants.TILE_SIZE;
		}

		_currentGroundPos.x += MisConstants.TILE_SIZE;
	}
		
	private Vector2 GetHeroRelativePos() {

		Vector2 relativePos = Vector2.zero;

		if (!MisGameWorld.Instance.WorldHero)
			return relativePos;

		Vector2 heroPos = MisGameWorld.Instance.WorldHero.transform.position;

		float rateX = Mathf.Floor(heroPos.x / (_lenght * MisConstants.TILE_SIZE) * _lenght);
		relativePos.x = rateX * MisConstants.TILE_SIZE;

		float rateY = Mathf.Floor(heroPos.y / (MisConstants.LEVEL_HEIGHT * MisConstants.TILE_SIZE) * MisConstants.LEVEL_HEIGHT);
		relativePos.y = rateY * MisConstants.TILE_SIZE;

		return relativePos;
	}

	public void BuildTile(int tileType, Vector2 position, Vector3 eulerAngles) {

		GameObject plat = _platforms [tileType];

		foreach (Transform tile in plat.transform) {

			Vector3 tilePos = (Vector3)position + tile.position;

			float xKey = float.Parse(tilePos.x.ToString("0.00"));
			float yKey = float.Parse(tilePos.y.ToString("0.00"));
			Vector2 dictKey = new Vector2 (xKey, yKey);  

			if (_collidebleTiles.ContainsKey (dictKey) &&
			    _collidebleTiles [dictKey].gameObject == null) {

				_collidebleTiles.Remove (dictKey);
			}

			if (!_collidebleTiles.ContainsKey (dictKey)) {

				GameObject freeTile = _tilesSource.GetFreeObject ();
				freeTile.transform.position = tilePos;

//				if (tileType != (int)PLATFORMS.TETRIS4) {
//					freeTile.transform.rotation = tile.rotation;
//					freeTile.transform.Rotate (eulerAngles);
//				}

				_collidebleTiles [dictKey] = freeTile;
				_tilesAdded.Add (freeTile);
			}
		}
	}

	void InitTile(GameObject obj) {

		obj.GetComponent<MisDestroyableObject> ().ObjectSource = _tilesSource;
	}

	void InitEnemy(GameObject obj) {

		obj.GetComponent<MisDestroyableObject> ().ObjectSource = _enemiesSource;
	}

	int NoiseToTile(float noiseValue) {

		if (noiseValue >= -1f && noiseValue < -0.6f)
			return (int)PLATFORMS.TETRIS1;

		if (noiseValue >= -0.6f && noiseValue < -0.2f)
			return (int)PLATFORMS.TETRIS2;

		if (noiseValue >= -0.2f && noiseValue < 0.4f)
			return (int)PLATFORMS.TETRIS3;

		if (noiseValue >= 0.4f && noiseValue < 0.8f)
			return (int)PLATFORMS.TETRIS4;

		return (int)PLATFORMS.TETRIS5;
	}

	int NoiseToTileHeight(float noiseValue) {

		if (noiseValue >= -1f && noiseValue < -0.4f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 7;

		if (noiseValue >= -0.4f && noiseValue < -0.2f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 6;

		if (noiseValue >= -0.2f && noiseValue < 0f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 5;

		if (noiseValue >= 0f && noiseValue < 0.2f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 4;

		if (noiseValue >= 0.2f && noiseValue < 0.4f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 3;

		if (noiseValue >= 0.4f && noiseValue < 0.6f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 2;

		if (noiseValue >= 0.6f && noiseValue < 0.8f)
			return MisConstants.LEVEL_GROUND_HEIGHT + 1;

		return MisConstants.LEVEL_GROUND_HEIGHT;
	}

	int NoiseToGroundHeight(float noiseValue) {

		if (noiseValue >= -2f && noiseValue < 0f)
			return MisConstants.LEVEL_GROUND_HEIGHT;

		if (noiseValue >= 0f && noiseValue < 0.25f)
			return MisConstants.LEVEL_GROUND_HEIGHT - 1;

		if (noiseValue >= 0.25f && noiseValue < 0.5f)
			return MisConstants.LEVEL_GROUND_HEIGHT - 2;

		return MisConstants.LEVEL_GROUND_HEIGHT - 3;
	}

	float NoiseToRotation(float noiseValue) {

		if (noiseValue >= -2f && noiseValue < 0f)
			return 0f;

		if (noiseValue >= 0f && noiseValue < 0.25f)
			return 90f;

		if (noiseValue >= 0.25f && noiseValue < 0.5f)
			return 180f;

		return 270f;
	}
}