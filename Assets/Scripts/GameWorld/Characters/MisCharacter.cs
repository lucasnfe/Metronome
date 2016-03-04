using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class MisCharacter : MisMoveableObject {

	protected bool  _isAttacking;

	protected virtual void Update() {

		if (_isDead)
			return;

		Vector3 constrainedPos = transform.position;

		constrainedPos.x = Mathf.Clamp (constrainedPos.x, MisGameWorld.Instance.WorldHorizontalConstraints.x, 
			MisGameWorld.Instance.WorldHorizontalConstraints.y);

		transform.position = constrainedPos;

//		UpdateState();
	}

	private void UpdateState() {

		if (_animator) {

			_animator.SetBool ("isRunning", IsRunning () && !_isAttacking);
			_animator.SetBool ("isJumping", (IsJumping () || IsFalling ()) && !_isAttacking);
			_animator.SetBool ("isShooting", _isAttacking);
		}
	}
}
