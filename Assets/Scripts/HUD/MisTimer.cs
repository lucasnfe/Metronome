using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class MisTimer : MonoBehaviour {

	public int startTime;
	private Text _timeText;

	// Use this for initialization
	void Start () {
	
		_timeText = GetComponent<Text> ();
		_timeText.text = FormatTime (startTime);

		InvokeRepeating ("DecreaseSecond", 0f, 1f);
	}

	string FormatTime(int time) {

		int minutes = time / 60; 
		int seconds = time % 60;

		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}

	void DecreaseSecond() {

		startTime--;
		_timeText.text = FormatTime (startTime);
	}
}
