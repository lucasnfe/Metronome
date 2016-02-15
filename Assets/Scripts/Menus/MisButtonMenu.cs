using UnityEngine;
using System.Collections;

public class MisButtonMenu : MonoBehaviour {
	
	// Use this for initialization
	public void LoadScene (string sceneName) {

		MisSceneManager.Instance.LoadScene (sceneName, true);
	}
}
