using UnityEngine;
using System.Collections;

public class MisMainMenu : MonoBehaviour {

	void LoadGameWorld() {

		MisGameWorld gw = MisGameWorld.Instance;
	}

	// Use this for initialization
	public void PlayGame () {
		MisSceneManager.Instance.LoadScene ("RandomLevel", true, LoadGameWorld);
	}
}
