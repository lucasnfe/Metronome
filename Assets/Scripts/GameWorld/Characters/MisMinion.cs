using UnityEngine;
using System.Collections;

public class MisMinion : MisEnemy {

	float _moveSide = -1f;

	protected override void Update() {

		base.Update ();

		if (_waitTimeAfterHit > 0f) {

			_waitTimeAfterHit -= Time.deltaTime;
			return;
		}

		if(_isOnGround)
			ApplyForce(Vector2.right * _moveSide * _moveSpeed * Time.deltaTime);
	}

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		base.DidEnterCollision (hit, normal);

		if (hit.tag == "Wall" && (normal == Vector2.right || normal == Vector2.left)  ) {

			_moveSide *= -1;
		}
	}
}
