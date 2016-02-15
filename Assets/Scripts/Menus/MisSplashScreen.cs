using UnityEngine;
using System.Collections;

public class MisSplashScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {

		MisSceneManager.Instance.LoadScene ("MainMenu");
	}
}
