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

		constrainedPos.y = Mathf.Clamp (constrainedPos.y, -Mathf.Infinity,  
			MisGameWorld.Instance.WorldVerticalConstraints.y);

		transform.position = constrainedPos;

		if (transform.position.y < MisGameWorld.Instance.WorldVerticalConstraints.x - 1f * MisConstants.TILE_SIZE)
			KillCharacter (false);

		UpdateState();
	}

	protected virtual void UpdateState() {

		if (_animator) {

			_animator.SetBool ("isRunning", IsRunning () && _isOnGround && !_isAttacking);
			_animator.SetBool ("isJumping", (IsJumping () || IsFalling ()) && !_isAttacking);
			_animator.SetBool ("isShooting", _isAttacking);
		}
	}
}
