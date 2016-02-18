using UnityEngine;
using System.Collections;

public class MisCharacter : MisMoveableObject {

	protected Animator _animator;

	protected bool     _isDead;
	protected bool     _isAttacking;
	
	// Use this for initialization
	protected override void Start () {

		base.Start ();

		_animator = GetComponent<Animator>();
	}

	protected virtual void Update() {

		_move = Vector2.zero;

		if (!_isDead)
			UpdateState();
	}

	private void UpdateState() {

		if (_animator) {

			_animator.SetBool ("isRunning", IsRunning () && !_isAttacking);
			_animator.SetBool ("isJumping", (IsJumping () || IsFalling ()) && !_isAttacking);
			_animator.SetBool ("isShooting", _isAttacking);
		}
	}
}
