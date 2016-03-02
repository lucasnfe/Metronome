using UnityEngine;
using System.Collections;

public class MisEnemy : MisCharacter {

	protected override void Update() {

		base.Update ();

		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("create"))
			return;

		if (MisGameWorld.Instance.GameHero) {

			Vector2 heroPos = MisGameWorld.Instance.GameHero.transform.position;
			Vector2 normalDirection = (heroPos - (Vector2)transform.position).normalized;

			_velocity = (normalDirection * _moveSpeed * Time.deltaTime);
		}
	}

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("create"))
			return;

		base.DidEnterEventCollision (hit, normal);
		HitTarget (hit, normal);
	}

	protected override void DidStayCollision(Collider2D hit, Vector2 normal) {

		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("create"))
			return;

		base.DidStayCollision (hit, normal);
		HitTarget (hit, normal);
	}

	private void HitTarget(Collider2D hit, Vector2 normal) {

		MisCharacter target = hit.transform.GetComponent<MisCharacter> ();
		if (target != null && target.tag == "Player") {

			if (target._isIndestructible)
				return;

			target.DealDamage (1);

			Vector2 normalDist = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;

			target.Velocity = Vector2.zero;
			target.ApplyForce (normalDist * 10f * Time.deltaTime);

			target.SetIndestructibleForTime(1f);
		}
	}
}
