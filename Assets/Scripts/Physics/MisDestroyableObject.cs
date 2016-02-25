using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (AudioSource))]
public class MisDestroyableObject : MonoBehaviour {

	protected Animator    _animator;
	protected AudioSource _audioSource;

	public int _life = 10;
	public int Life { get { return _life; } }

	public AudioClip [] _soundEffects;

	void Start() {

		_animator = GetComponent<Animator> ();
		_audioSource = GetComponent<AudioSource> ();
	}

	public void DealDamage(int damage) {

		_life -= damage;
		PlaySFX (DESTROY_SFX.HIT);

		_animator.SetTrigger ("hit");

		if (_life <= 0) {

			PlaySFX (DESTROY_SFX.DESTROY);
			Invoke ("SelfDestroy", 0.05f);
		}
	}

	public void PlaySFX(DESTROY_SFX clip) {

		_audioSource.PlayOneShot (_soundEffects [(int)clip]);
	}

	void SelfDestroy() {

		Destroy (gameObject);
	}
}
