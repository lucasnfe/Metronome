using UnityEngine;
using System.Collections;

public class MisWorldMap : MonoBehaviour {
	
	void LoadGameWorld() {

		MisGameWorld gw = MisGameWorld.Instance;
	}

	void OnMouseDown() 
	{
		MisSceneManager.Instance.LoadScene ("RandomLevel", true, LoadGameWorld);
	}
}
