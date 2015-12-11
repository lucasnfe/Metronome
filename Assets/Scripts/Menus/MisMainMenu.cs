using UnityEngine;
using System.Collections;

public class MisMainMenu : MonoBehaviour {
	
	// Use this for initialization
	public void PlayGame () {
		MisSceneManager.Instance.LoadScene ("WorldMap", true);
	}
}
