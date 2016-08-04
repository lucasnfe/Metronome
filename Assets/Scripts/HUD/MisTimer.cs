using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class MisTimer : MonoBehaviour {

	public int     _startTime;
	private float _displayTime;

	public float  DisplayTime  { get { return _displayTime; } }
	public bool   Pause        { get; set; }

	private Text   _timeText;

	// Use this for initialization
	void Start () {

		_timeText = GetComponent<Text> ();
		SetTimer (_startTime);

		Pause = true;
	}

	void FixedUpdate() {

		if (Pause)
			return;

		if (_displayTime > 0) {

			_displayTime -= Time.fixedDeltaTime * 1.66f;
			_timeText.text = FormatTime (_displayTime);
		}
	}


	string FormatTime(float time) {

		int minutes = (int)time / 60; 
		int seconds = (int)time % 60;

		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}

	public void SetTimer(int time) {

		_displayTime = time;
		_timeText.text = FormatTime (_displayTime);
	}
}
