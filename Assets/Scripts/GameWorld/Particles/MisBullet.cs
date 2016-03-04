using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class MisBullet : MisMoveableObject {
	
	float _dir = 0f;

	public MisGun firedGun { get ; set; }

	void Update () {

		ApplyForce (Vector2.right * _moveSpeed * _dir * Time.fixedDeltaTime);

		if (!_renderer.isVisible)
			firedGun.DestroyBullet(this);
	}

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		base.DidEnterEventCollision (hit, normal);
		HitTarget (hit);
	}

	protected override void DidStayCollision(Collider2D hit, Vector2 normal) {

		base.DidStayCollision (hit, normal);
		HitTarget (hit);
	}

	private void HitTarget(Collider2D hit) {

		if (firedGun.owner == null)
			return;

		MisDestroyableObject target = hit.transform.GetComponent<MisDestroyableObject> ();

		if (target != null && target.tag != firedGun.owner.tag)
			target.DealDamage (firedGun.damage);

		if(hit.tag != firedGun.owner.tag)
			firedGun.DestroyBullet (this);
	}

	public void SetDirection(float dir) {

		_dir = Mathf.Sign (dir);
	}
}