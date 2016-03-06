using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class MisTimer : MisSingleton<MisTimer> {

	public int  startTime;

	public bool  Pause       { get; set; }
	public float DisplayTime { get; set; }

	private Text   _timeText;

	// Use this for initialization
	void Start () {
	
		DisplayTime = startTime;

		_timeText = GetComponent<Text> ();
		_timeText.text = FormatTime (DisplayTime);

		Pause = true;
	}

	void FixedUpdate() {

		if (Pause)
			return;

		if (DisplayTime > 0) {

			DisplayTime -= Time.deltaTime;
			_timeText.text = FormatTime (DisplayTime);
		}
	}

	string FormatTime(float time) {

		int minutes = (int)time / 60; 
		int seconds = (int)time % 60;

		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}
}
