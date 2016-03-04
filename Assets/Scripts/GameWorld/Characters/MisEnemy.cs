using UnityEngine;
using System.Collections;

public class MisEnemy : MisCharacter {

	float _waitTimeAfterHit;
	float _moveSide = -1f;

	protected override void Update() {

		base.Update ();

		if (_waitTimeAfterHit > 0f) {

			_waitTimeAfterHit -= Time.deltaTime;
			return;
		}

		ApplyForce(Vector2.right * _moveSide * _moveSpeed * Time.deltaTime);
	}

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		base.DidEnterCollision (hit, normal);

		if (hit.tag == "Player") {

			HitTarget (hit, normal);
		} 
		else if (hit.tag == "Wall" && (normal == Vector2.right || normal == Vector2.left)  ) {

			_moveSide *= -1;
		}
	}

	private void HitTarget(Collider2D hit, Vector2 normal) {

		MisCharacter target = hit.transform.GetComponent<MisCharacter> ();
		if (target != null) {

			if (target._isIndestructible)
				return;

			Velocity = Vector2.zero;

			if (transform.position.x > hit.transform.position.x) {

				target.ApplyForce (Vector2.right * -30f * Time.deltaTime);
				ApplyForce (Vector2.right * 30f * Time.deltaTime);
			} 
			else {

				target.ApplyForce (Vector2.right * 30f * Time.deltaTime);
				ApplyForce (Vector2.right * -30f * Time.deltaTime);
			}

			target.Velocity = Vector2.zero;
			target.DealDamage (1);
			target.SetIndestructibleForTime(0.2f);

			_waitTimeAfterHit = 1f;
		}
	}

	void TurnDirection() {


	}
}
