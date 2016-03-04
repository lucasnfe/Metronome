using UnityEngine;
using System.Collections;
using SynchronizerData;

public class MetronomeLevelGenerator : MisLevelGenerator {

	private float _placementTimer;

	private Vector2    _currentGroundPos;
	private Vector2    _platformPlacementVelocity;
	private GameObject _currentPlatform;

	private BeatObserver     _beatObserver;
	private BeatSynchronizer _beatSync;

	public static readonly int FST_SECTION = 8;
	public static readonly int SND_SECTION = 16;
	public static readonly int FST_JMP_SIZE = 4;
	public static readonly int PLAT1_POS = 2;
	public static readonly int PLAT2_POS = 12;

	private bool _addedGap;

	protected override void Awake() {

		base.Awake ();

		_beatObserver = GetComponent<BeatObserver>();

		_beatSync = GetComponent<BeatSynchronizer>();
		_beatSync.enabled = false;

		_platformPlacementVelocity = new Vector2 (0f, -10f);
	}

	void FixedUpdate() {

		if ((int)(_currentGroundPos.x / MisConstants.TILE_SIZE) < _lenght) {
			
			if (_beatSync.enabled) {

				_placementTimer += Time.fixedDeltaTime * 6.1f;
				if (_placementTimer >= 1f) {

					PlaceGround ();
					_placementTimer = 0f;
				}
			}
		}
	}

	void Update() {

		if ((_beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat) {

			if ((int)(_currentGroundPos.x / MisConstants.TILE_SIZE) < _lenght)
				AddPlatform ();
		}

		if ((_beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat) {

				AddEnemy ();
		}				
	}
		
	protected override void GenerateLevel(Vector2 startPos, int levelLenght) {

		_currentGroundPos = startPos;

		for (int i = 0; i < FST_SECTION; i++) {

			GameObject newPlat = BuildTile (_currentGroundPos, _level.transform, _platforms [(int)PLATFORMS.STATIC]);
			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT);
			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
		}
			
		_currentGroundPos.x +=  MisConstants.TILE_SIZE * FST_JMP_SIZE;
		_currentGroundPos.y +=  MisConstants.TILE_SIZE * FST_JMP_SIZE;

		for (int i = 0; i < SND_SECTION; i++) {

			if (i == PLAT1_POS) {

				GameObject eventPlat = BuildTile (_currentGroundPos + Vector2.up * MisConstants.TILE_SIZE, 
					_level.transform, _platforms [(int)PLATFORMS.EVENT]);
				CreateEventPlatform (eventPlat, AddFirstPlatform);
			}

			if (i == PLAT2_POS) {

				GameObject metroPlat = BuildTile (_currentGroundPos + Vector2.up * MisConstants.TILE_SIZE, _level.transform, _platforms [(int)PLATFORMS.EVENT]);
				CreateEventPlatform (metroPlat, StartMetronome);
			}

			GameObject newPlat = BuildTile (_currentGroundPos, _level.transform, _platforms [(int)PLATFORMS.STATIC]);
			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT + FST_JMP_SIZE);

			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
		}

		_currentGroundPos.y -= MisConstants.TILE_SIZE * FST_JMP_SIZE;
	} 

	// Fullfill area below the surface with background tile
	private void FulfillGround(GameObject groundPlat, int height) {

		Vector2 startPos = groundPlat.transform.position;

		for(int j = 0; j < height; j++) {

			Vector2 pos = new Vector2 (startPos.x, startPos.y - (j + 1) *  MisConstants.TILE_SIZE);
			BuildTile (pos, _level.transform, _platforms [(int)PLATFORMS.STATIC]);
		}
	}

	private void CreateEventPlatform(GameObject surface, MisEventPlatform.PlatformEvent platEvent, int eventLimit = 1) {

		MisEventPlatform killZone = surface.AddComponent<MisEventPlatform> ();

		killZone.EventLimit = eventLimit;
		killZone.Normal = new Vector2 (-1f, 0f);
		killZone.Event = platEvent;

		surface.tag = PLATFORMS.EVENT.ToString();
	}
		
	private void AddFirstPlatform() {
		
		Vector2 relativ = GetHeroRelativePos ();
		Vector2 platPos = relativ + new Vector2(4f, 6f) * MisConstants.TILE_SIZE;

		_currentPlatform = BuildTile (platPos, _level.transform, _platforms [(int)PLATFORMS.TETRIS1]);
	
		GameObject enemy = Instantiate (_enemies [(int)ENEMIES.BAT]);
		enemy.name = _platforms [(int)PLATFORMS.TETRIS1].name;
		enemy.transform.position = platPos + Vector2.up * 2f * MisConstants.TILE_SIZE;
		enemy.transform.parent = _level.transform;

	}
		
	private void AddPlatform() {

		Vector2 platPos = GetHeroRelativePos () + new Vector2(4f, Random.Range(1, 8)) * MisConstants.TILE_SIZE;

		int randomPlat = Random.Range((int)PLATFORMS.TETRIS1, (int)PLATFORMS.TETRIS6 + 1);
		_currentPlatform = BuildTile (platPos, _level.transform, _platforms [randomPlat]);
	}

	private void AddEnemy() {

		Vector3 platPos = GetHeroRelativePos () + new Vector2(4f, Random.Range(1, 8)) * MisConstants.TILE_SIZE;
		platPos.z = _enemies [(int)ENEMIES.BAT].transform.position.z;

		GameObject enemy = Instantiate (_enemies [(int)ENEMIES.BAT]);
		enemy.name = _enemies [(int)ENEMIES.BAT].name;

		enemy.transform.parent = _level.transform;
		enemy.transform.position = platPos;
	}

	private void StartMetronome() {

		_beatSync.enabled = true;
		_placementTimer = 1f;
		MisTimer.Instance.Pause = false;
	}

	private void PlaceGround() {

		if (_addedGap) {

			GameObject newPlat = BuildTile (_currentGroundPos, _level.transform, _platforms [(int)PLATFORMS.STATIC]);
			FulfillGround (newPlat, MisConstants.LEVEL_GROUND_HEIGHT);
			_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;

			_addedGap = false;

			return;
		}

		if (!_addedGap && Random.value > 0.9f) {

			_addedGap = true;
			_currentGroundPos += Vector2.right * Random.Range(3, 4) * MisConstants.TILE_SIZE;
			return;
		}
			
		GameObject newPlat1 = BuildTile (_currentGroundPos, _level.transform, _platforms [(int)PLATFORMS.STATIC]);
		FulfillGround (newPlat1, MisConstants.LEVEL_GROUND_HEIGHT);
		_currentGroundPos += Vector2.right *  MisConstants.TILE_SIZE;
	}
		
	private Vector2 GetHeroRelativePos() {

		Vector2 relativePos = Vector2.zero;

		if (!MisGameWorld.Instance.WorldHero)
			return relativePos;

		Vector2 heroPos = MisGameWorld.Instance.WorldHero.transform.position;

		float rate = Mathf.Floor(heroPos.x / (_lenght * MisConstants.TILE_SIZE) * 1000f);
		relativePos.x = rate * MisConstants.TILE_SIZE;
		relativePos.y = (int)_currentGroundPos.y;

		return relativePos;
	}
}