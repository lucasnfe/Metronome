using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SynchronizerData;

public class MetronomeLevelGenerator : MisLevelGenerator {

	private float []_spectrum;

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

	public static readonly int FST_SECTION = 8;
	public static readonly int SND_SECTION = 16;
	public static readonly int SND_SECTION_HEIGHT = 4;
	public static readonly int FST_JMP_SIZE = 4;
	public static readonly int PLAT1_POS = 3;
	public static readonly int PLAT2_POS = 12;
	public static readonly float GROUND_PLACEMENT_TIME = 6.5f;

	private bool _addedGap;

	protected override void Start() {

		base.Start ();

		_beatSync = GetComponent<BeatSynchronizer>();
		_beatCounter = GetComponent<BeatCounter>();
		_beatObserver = GetComponent<BeatObserver>();

		_beatSync.enabled = false;

		_tilesSource  = new MisObjectPool (_platforms[(int)PLATFORMS.BREAKABLE], 300, transform);
		_tilesSource.ExecActionInObjects (InitTile);

		_enemiesSource  = new MisObjectPool (_enemies[(int)ENEMIES.BAT], 50, transform);
		_enemiesSource.ExecActionInObjects (InitEnemy);

		_tilesAdded    = new List<GameObject> ();
		_tilesToDelete = new List<GameObject> ();

		_noiseGenerator = new PerlinNoise (0);

		_lenght = (int)(_beatCounter.audioSource.clip.length * 8f);
		_lenght += FST_SECTION + SND_SECTION + FST_JMP_SIZE;

		_spectrum = new float[64];
	}

	void Update() {

		if ((_beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat) {

			if ((int)(_currentGroundPos.x / MisConstants.TILE_SIZE) < _lenght) {

				AddPlatform ();
				PlaceGround ();
			}
		}

		if ((_beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat) {

			if ((int)(_currentGroundPos.x / MisConstants.TILE_SIZE) < _lenght) {

				AddEnemy (_currentPlarfoPos);
				AddEnemy (_currentGroundPos);
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
		
	protected override void GenerateLevel(Vector2 startPos, int levelLenght) {

		_currentGroundPos = startPos + Vector2.up * MisConstants.LEVEL_GROUND_HEIGHT * MisConstants.TILE_SIZE;

		for (int i = 0; i < FST_SECTION; i++) {

			GameObject newPlat = (GameObject)Instantiate (_platforms [(int)PLATFORMS.STATIC], 
				_currentGroundPos,Quaternion.identity);
			newPlat.transform.parent = _level.transform;

			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT);
			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
		}
			
		_currentGroundPos.x +=  MisConstants.TILE_SIZE * FST_JMP_SIZE;
		_currentGroundPos.y +=  MisConstants.TILE_SIZE * SND_SECTION_HEIGHT;

		for (int i = 0; i < SND_SECTION; i++) {

			if (i == PLAT1_POS) {

				GameObject firstPlat1 = (GameObject) Instantiate (_platforms [(int)PLATFORMS.TETRIS1], 
					_currentGroundPos + Vector2.up * 2f * MisConstants.TILE_SIZE, Quaternion.identity);
				firstPlat1.transform.parent = _level.transform;

				GameObject firstPlat2 = (GameObject) Instantiate (_platforms [(int)PLATFORMS.TETRIS1], 
					_currentGroundPos + Vector2.up * 5f * MisConstants.TILE_SIZE, Quaternion.identity);
				firstPlat2.transform.parent = _level.transform;

				GameObject firstEnemy = (GameObject) Instantiate (_enemies [(int)ENEMIES.BAT], 
					_currentGroundPos + Vector2.one * 1f * MisConstants.TILE_SIZE, Quaternion.identity);
				firstEnemy.transform.parent = _level.transform;
			}

			if (i == PLAT2_POS) {

				GameObject metroPlat = (GameObject) Instantiate (_platforms [(int)PLATFORMS.EVENT],
					_currentGroundPos + Vector2.up * MisConstants.TILE_SIZE, Quaternion.identity);
				metroPlat.transform.parent = _level.transform;

				CreateEventPlatform (metroPlat, StartMetronome);
			}

			GameObject newPlat = (GameObject) Instantiate (_platforms [(int)PLATFORMS.STATIC],
				_currentGroundPos, Quaternion.identity);
			newPlat.transform.parent = _level.transform;

			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT + SND_SECTION_HEIGHT);

			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
		}

		_currentGroundPos.y -= MisConstants.TILE_SIZE * (SND_SECTION_HEIGHT + 2f);
	} 

	// Fullfill area below the surface with background tile
	private void FulfillGround(GameObject groundPlat, int height) {

		Vector2 startPos = groundPlat.transform.position;

		for(int j = 0; j < height; j++) {

			Vector2 pos = new Vector2 (startPos.x, startPos.y - (j + 1) *  MisConstants.TILE_SIZE);

			GameObject newPlat = (GameObject) Instantiate (_platforms [(int)PLATFORMS.STATIC],pos,Quaternion.identity);
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

		float x = _spectrum [_spectrum.Length / 2];
		float noiseTile = _noiseGenerator.FractalNoise1D (x, 10, _lenght * 0.0000001f, 2f);
		float noisePos  = _noiseGenerator.FractalNoise1D (x, 10, _lenght * 0.0000001f, 2f);
		float noiseRot  = _noiseGenerator.FractalNoise1D (x, 10, _lenght * 0.0000001f, 2f);

		Vector2 platPos = new Vector2(_currentGroundPos.x, NoiseToTileHeight(noisePos) * MisConstants.TILE_SIZE);
		BuildTile (NoiseToTile(noiseTile), platPos, Vector3.forward * NoiseToRotation(noiseRot));
		_currentPlarfoPos = platPos;
	}

	private void AddEnemy(Vector3 referencePos) {

		Vector3 platPos = referencePos + Vector3.up * 2f * MisConstants.TILE_SIZE;
		platPos.z = _enemies [(int)ENEMIES.BAT].transform.position.z;

		GameObject enemy = _enemiesSource.GetFreeObject();
		enemy.transform.parent = _level.transform;
		enemy.transform.position = platPos;
	}

	private void StartMetronome() {

		_beatSync.enabled = true;
		MisTimer.Instance.Pause = false;
	}

	private void PlaceGround() {

		_beatCounter.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

		float x = _spectrum [_spectrum.Length / 2];
		float noisePos = _noiseGenerator.FractalNoise1D (x, 10, _lenght * 0.0000001f, 2f);

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

			GameObject freeTile = _tilesSource.GetFreeObject ();
			freeTile.transform.position = (Vector3)position + tile.position;
			freeTile.transform.rotation = tile.rotation;

			freeTile.transform.Rotate (eulerAngles);

			_tilesAdded.Add (freeTile);
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