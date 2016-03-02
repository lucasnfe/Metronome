using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class MisCharacter : MisMoveableObject {

	protected bool  _isDead;
	protected bool  _isAttacking;

	protected virtual void Update() {

		if (!_isDead)
			UpdateState();
	}

	private void UpdateState() {

		if (_animator) {

//			_animator.SetBool ("isRunning", IsRunning () && !_isAttacking);
//			_animator.SetBool ("isJumping", (IsJumping () || IsFalling ()) && !_isAttacking);
//			_animator.SetBool ("isShooting", _isAttacking);
		}
	}
}
