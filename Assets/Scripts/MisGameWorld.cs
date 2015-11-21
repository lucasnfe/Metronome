using UnityEngine;
using System.Collections;

public class MisGameWorld : MisSingleton<MisGameWorld> {
		
	private GameObject   	  _heroPrefab;
	private GameObject      []_enemiesPrefab;

	private MisCamera          _misCamera;
	private MisGroundGenerator _misLevelGenerator;
	private MisAudioManager    _misAudioManager;
	private MisHero 		   _misHero;

	private Vector2 		   _nextSpawningPoint;
	private GameObject         _ground;

	// Use this for initialization
	void Start () {

		DontDestroyOnLoad (gameObject);
	
		_misLevelGenerator = FindObjectOfType (typeof(MisGroundGenerator)) as MisGroundGenerator;
		if (!_misLevelGenerator) {

			GameObject temp = Instantiate(Resources.Load("MisLevelGenerator") as GameObject);
			_misLevelGenerator = temp.GetComponent<MisGroundGenerator>();
		}

		DontDestroyOnLoad (_misLevelGenerator.gameObject);

		_misAudioManager = FindObjectOfType (typeof(MisAudioManager)) as MisAudioManager;
		if (!_misAudioManager) {

			GameObject temp = Instantiate(Resources.Load("MisAudioManager") as GameObject);
			_misAudioManager = temp.GetComponent<MisAudioManager>();
		}

		_heroPrefab = Resources.Load("Characters/MisHero") as GameObject;

		DontDestroyOnLoad (_misAudioManager.gameObject);
	}

	void OnLevelWasLoaded(int level) {
	
		switch (level) {

		case 1:
			LoadTransictionScreen();
			break;
		
		default:
			LoadPlayableLevel();
			break;
		}
	}

	void LoadTransictionScreen() {

		if (_ground)
			_ground.SetActive (false);
	}

	void LoadPlayableLevel() {

		if (_ground)
			_ground.SetActive (true);
		
		_misCamera = FindObjectOfType (typeof(MisCamera)) as MisCamera;
		if (!_misCamera) {
			
			Debug.LogError ("You must define a camera!");
			return;
		}
		
		if (!_ground) {
			
			_ground = _misLevelGenerator.GenerateLevel ();
			_ground.transform.parent = transform;

			DontDestroyOnLoad (_ground);
		} else
			_ground.SetActive (true);
		
		_misAudioManager.PlayBacktrack ();
		
		if(_misHero)
			Destroy(_misHero.gameObject);
		
		SpawnHero (_nextSpawningPoint);
	}

	public void ResetLevel(Vector2 heroSpawningPoint) {

		Destroy(_misHero.gameObject);

		_nextSpawningPoint = heroSpawningPoint;
		MisSceneManager.Instance.LoadScene (Application.loadedLevelName);
	}

	void SpawnHero(Vector2 spawningPoint) {

		float zPos = _heroPrefab.transform.position.z;
		Vector3 spawningPos = new Vector3(spawningPoint.x, spawningPoint.y, zPos);

		GameObject heroObj = Instantiate (_heroPrefab, spawningPos, Quaternion.identity) as GameObject;
		heroObj.name = MisConstants.HERO_NAME;
		heroObj.transform.parent = transform;
		
		_misHero = heroObj.GetComponent<MisHero>();
		
		if (_misCamera) {
			_misCamera._player = _misHero.gameObject;
			_misCamera.Move(_misHero.transform.position);
		}
	}
}
