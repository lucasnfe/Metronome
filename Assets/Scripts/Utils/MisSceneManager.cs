using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (Camera))]
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

		GameObject _loadingBanner = null;
		if (showLoadingScreen)
			SceneManager.LoadScene ("LoadingScene");

		yield return new WaitForSeconds(0.1f);

		if (actioneBetweenScenes != null)
			actioneBetweenScenes();

		if (showLoadingScreen) {
			_loadingBanner = Camera.main.gameObject;
			DontDestroyOnLoad (_loadingBanner);
		}

		SceneManager.LoadScene(sceneName);

		yield return new WaitForSeconds(0.1f);

		_currentScene = SceneManager.GetActiveScene ().buildIndex;

		if (actioneAfterLoading != null)
			actioneAfterLoading();

		if (_loadingBanner)
			Destroy (_loadingBanner);

		if (sceneName.EndsWith("Menu")) {
			if(!MisAudioController.Instance.IsPlayingMusic(_backgroundMusic))
				MisAudioController.Instance.PlayMusic(_backgroundMusic);
		}
		else
			MisAudioController.Instance.StopMusic();
	}
}
