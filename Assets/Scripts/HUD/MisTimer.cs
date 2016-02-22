using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class MisTimer : MonoBehaviour {

	public int startTime;

	private float  _timeRemaining;
	private Text _timeText;

	// Use this for initialization
	void Start () {
	
		_timeRemaining = startTime;

		_timeText = GetComponent<Text> ();
		_timeText.text = FormatTime (_timeRemaining);
	}

	void FixedUpdate() {

		if (_timeRemaining > 0) {
			
			_timeRemaining -= Time.deltaTime * 2f;
			_timeText.text = FormatTime (_timeRemaining);
		}
	}

	string FormatTime(float time) {

		int minutes = (int)time / 60; 
		int seconds = (int)time % 60;

		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}
}
