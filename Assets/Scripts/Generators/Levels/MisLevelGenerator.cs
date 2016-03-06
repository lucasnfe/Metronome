using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisLevelGenerator : MonoBehaviour {

	public GameObject[] _platforms;
	public GameObject[] _enemies;

	public int         _lenght = 1;
	public Vector2     _startPos;

	protected GameObject _level;

	// Use this for initialization
	protected virtual void Start () {

	}

	public GameObject GenerateLevel() {

		_level = new GameObject();
		_level.name = "Level";

		GenerateLevel (_startPos, _lenght);

		return _level;
	}

	protected virtual void GenerateLevel(Vector2 startPos, int levelLenght) {

	}
}