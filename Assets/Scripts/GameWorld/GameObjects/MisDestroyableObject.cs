using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (AudioSource))]
[RequireComponent (typeof (BoxCollider2D))]
public class MisDestroyableObject : MonoBehaviour {

	public MisObjectPool ObjectSource { get; set; }

	protected Animator       _animator;
	protected AudioSource    _audioSource;
	protected SpriteRenderer _renderer;
	protected BoxCollider2D  _boundingBox;

	protected bool _isDead;

	public int  _life = 1;
	public bool _isIndestructible;

	public AudioClip [] _soundEffects;

	protected virtual void Start() {

		_animator = GetComponent<Animator> ();
		_audioSource = GetComponent<AudioSource> ();
		_renderer = GetComponent<SpriteRenderer>();
		_boundingBox = GetComponent<BoxCollider2D>();

		_isIndestructible = false;
	}

	public void DealDamage(int damage) {

		if (_isIndestructible)
			return;

		_life -= damage;
		PlaySFX ((int)DESTROY_SFX.HIT);

		_animator.SetTrigger ("hit");

		if (_life <= 0) {

			_isDead = true;
			KillCharacter ();
		}
	}

	public void PlaySFX(int clip) {

		if(_soundEffects.Length >= clip)
			_audioSource.PlayOneShot (_soundEffects [clip]);
	}

	public void SetIndestructibleForTime(float seconds) {

		_isIndestructible = true;

		Color currentColor = _renderer.color;
		currentColor.a = 0.5f;
		_renderer.color = currentColor;

		Invoke ("SetDestructible", seconds);
	}
		
	void SetDestructible() {

		_isIndestructible = false;
		Color currentColor = _renderer.color;
		currentColor.a = 1f;
		_renderer.color = currentColor;
	}

	public void DisableCollision() {

		_boundingBox.enabled = false;
	}

	public void EnableCollision() {

		_boundingBox.enabled = true;
	}

	protected void KillCharacter() {

		PlaySFX ((int)DESTROY_SFX.DESTROY);
		Invoke ("SelfDestroy", 0.05f);
	}
		
	void SelfDestroy() {

		if (ObjectSource != null)
			
			ObjectSource.SetFreeObject (this.gameObject);
		else
			Destroy (gameObject);
	}
}
