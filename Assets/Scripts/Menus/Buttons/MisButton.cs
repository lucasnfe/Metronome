using UnityEngine;
using System.Collections;

public class MisButton : MonoBehaviour {

	// Use this for initialization
	public void LoadScene (string sceneName) {

		MisSceneManager.Instance.LoadScene (sceneName, true, ActionBeween, ActionAfter);
	}

	protected virtual void ActionBeween () {
		
	}

	protected virtual void ActionAfter () {

	}
}