using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (AudioSource))]
public class MisCharacter : MisMoveableObject {

	protected Animator       _animator;
	protected SpriteRenderer _renderer;
	protected AudioSource    _audio;

	protected bool  _isDead;
	protected bool  _isAttacking;

	public AudioClip[] _soundEffects;
	
	// Use this for initialization
	protected override void Start () {

		base.Start ();

		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();
		_audio = GetComponent<AudioSource>();
	}

	protected virtual void Update() {

		if (!_isDead)
			UpdateState();
	}

	public void PlaySFX(CHARACTER_SFX clip) {

		_audio.PlayOneShot (_soundEffects [(int)clip]);
	}

	private void UpdateState() {

		if (_animator) {

			_animator.SetBool ("isRunning", IsRunning () && !_isAttacking);
			_animator.SetBool ("isJumping", (IsJumping () || IsFalling ()) && !_isAttacking);
			_animator.SetBool ("isShooting", _isAttacking);
		}
	}
}
