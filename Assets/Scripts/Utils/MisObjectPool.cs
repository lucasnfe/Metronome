using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisObjectPool {

	private List<GameObject> _pool;
	public int _polledAmount = 10;

	public delegate void InitObject(GameObject obj);

	// Use this for initialization
	public MisObjectPool (GameObject objTemplate, InitObject initMethod = null) {

		_pool = new List<GameObject> ();

		GameObject poolObj = new GameObject();
		poolObj.name = objTemplate.name + "Pool";

		for(int i = 0; i < _polledAmount; i++) {

			GameObject obj = GameObject.Instantiate (objTemplate);
			obj.name = "PolledObj_" + i;
			obj.transform.parent = poolObj.transform;
			obj.SetActive (false);

			if(initMethod != null) initMethod (obj);

			_pool.Add (obj);
		}
	}

	public GameObject GetFreeObject() {

		for(int i = 0; i < _polledAmount; i++) {

			if (!_pool [i].activeInHierarchy) {

				_pool [i].SetActive(true);
				return _pool [i];
			}
		}

		return null;
	}

	public void SetFreeObject(GameObject obj) {

		for(int i = 0; i < _polledAmount; i++) {

			if (obj.name == "PolledObj_" + i) {

				_pool [i].SetActive(false);
				return;
			}
		}
	}
}
