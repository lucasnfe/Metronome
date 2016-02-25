using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class MisBullet : MisMoveableObject {
	
	SpriteRenderer _renderer;
	float _dir = 0f;

	public MisGun firedGun { get ; set; }

	// Use this for initialization
	protected override void Start () {

		base.Start ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

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

		MisDestroyableObject target = hit.transform.GetComponent<MisDestroyableObject> ();
		if (target != null)
			target.DealDamage (firedGun.damage);

		firedGun.DestroyBullet(this);
	}

	public void SetDirection(float dir) {

		_dir = Mathf.Sign (dir);
	}
}