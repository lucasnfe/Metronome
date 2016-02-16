using UnityEngine;
using System.Collections;

public class MisButton : MonoBehaviour {

	// Use this for initialization
	public void LoadScene (string sceneName) {

		MisSceneManager.Instance.LoadScene (sceneName, true, LoadAction);
	}

	protected virtual void LoadAction () {
		
	}
}