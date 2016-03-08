using UnityEngine;
using System.Collections;

public class MisEnemy : MisCharacter {

	private float _separationForceX = 30f;
	private float _separationForceY = 2f;
	protected float _waitTimeAfterHit;

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		base.DidEnterCollision (hit, normal);

		if (hit.tag == "Player") {

			HitTarget (hit, normal);
		} 
	}

	public void HitTarget(Collider2D hit, Vector2 normal) {

		MisCharacter target = hit.transform.GetComponent<MisCharacter> ();
		if (target != null) {

			if (target._isIndestructible)
				return;

			Velocity = Vector2.zero;
			SetInvisibleForTime (0.2f);

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

	public void SetInvisibleForTime(float seconds) {

		_boundingBox.isTrigger = true;
		Invoke ("SetVisible", seconds);
	}

	void SetVisible() {

		_boundingBox.isTrigger = false;
	}
}
