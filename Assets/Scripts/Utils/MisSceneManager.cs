using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MisSceneManager : MisSingleton<MisSceneManager> {

	public delegate void ActionBetweenScenes();

	public AudioClip _backgroundMusic;

	private int _lastScene;
	public int LastScene { get { return _lastScene; } }

	private int _currentScene;
	public int CurrentScene { get { return _currentScene; } }

	void Start() {

		_backgroundMusic = Resources.Load("Audio/title_theme") as AudioClip;
	}

	public void LoadScene (string sceneName, bool showLoadingScreen = true, ActionBetweenScenes actioneBetweenScenes = null, ActionBetweenScenes actioneAfterLoading = null) {

		StartCoroutine(SceneSwitchCoroutine(sceneName, showLoadingScreen, actioneBetweenScenes, actioneAfterLoading));
	}

	public void ReloadScene (bool showLoadingScreen = true, ActionBetweenScenes actioneBetweenScenes = null, ActionBetweenScenes actioneAfterLoading = null) {

		string currentScene = SceneManager.GetActiveScene ().name;
		StartCoroutine(SceneSwitchCoroutine(currentScene, showLoadingScreen, actioneBetweenScenes, actioneAfterLoading));
	}

	IEnumerator SceneSwitchCoroutine (string sceneName, bool showLoadingScreen, ActionBetweenScenes actioneBetweenScenes, ActionBetweenScenes actioneAfterLoading) {

		_lastScene = SceneManager.GetActiveScene ().buildIndex;

		if(showLoadingScreen) 
			SceneManager.LoadScene("LoadingScene");

		yield return new WaitForSeconds(0.1f);

		if (actioneBetweenScenes != null)
			actioneBetweenScenes();

		SceneManager.LoadScene(sceneName);

		_currentScene = SceneManager.GetActiveScene ().buildIndex;

		yield return new WaitForSeconds(0.1f);

		if (actioneAfterLoading != null)
			actioneAfterLoading();

		if (sceneName.EndsWith("Menu")) {
			if(!MisAudioController.Instance.IsPlayingMusic(_backgroundMusic))
				MisAudioController.Instance.PlayMusic(_backgroundMusic);
		}
		else
			MisAudioController.Instance.StopMusic();
	}
}
