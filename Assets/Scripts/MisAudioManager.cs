using UnityEngine;
using System.Collections;

public class MisAudioManager : MonoBehaviour {

	private AudioSource _source;

	public void PlayBacktrack() {

		_source = GetComponent<AudioSource> ();

		if (!_source) return;

		if (!_source.isPlaying)
			_source.Play ();
	}
}
