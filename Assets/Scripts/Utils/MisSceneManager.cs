using UnityEngine;
using System.Collections;

public class MisSceneManager : MisSingleton<MisSceneManager> {

	public AudioClip _backgroundMusic;
	
	private int _lastScene;
	public int LastScene {
		get { return _lastScene; }
	}
	
	void Start() {
		
//		_backgroundMusic = Resources.Load("title_theme") as AudioClip;
	}
	
	public void LoadScene(string sceneName, bool showLoadingScreen = true, System.Action actioneBetweenScenes = null) {
		
		MisSceneManager.Instance.StartCoroutine(SceneSwitchCoroutine(sceneName, showLoadingScreen, actioneBetweenScenes));
	}
	
	IEnumerator SceneSwitchCoroutine(string sceneName, bool showLoadingScreen, System.Action actioneBetweenScenes) {
		_lastScene = Application.loadedLevel;
		if(showLoadingScreen) 
			Application.LoadLevel("LoadingScene");
		
		yield return new WaitForSeconds(0.01f);
		
		if (actioneBetweenScenes != null)
			actioneBetweenScenes();
		
		Application.LoadLevel(sceneName);
	}
}

