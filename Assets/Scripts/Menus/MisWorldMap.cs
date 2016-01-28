using UnityEngine;
using System.Collections;

public class MisWorldMap : MonoBehaviour {
	
	void LoadGameWorld() {

		MisGameWorld gw = MisGameWorld.Instance;
	}

	void Update() {

		if (Input.GetMouseButton (0)) {
			
			MisSceneManager.Instance.LoadScene ("RandomLevel", true, LoadGameWorld);
		}
	}
}
