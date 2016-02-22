using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class MisBullet : MisMoveableObject {
	
	SpriteRenderer _renderer;

	public MisGun firedGun { get ; set; }

	// Use this for initialization
	protected override void Start () {

		base.Start ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	protected override void FixedUpdate () {

		base.FixedUpdate ();

		if (!_renderer.isVisible)
			firedGun.DestroyBullet(this);
	}

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		base.DidEnterEventCollision (hit, normal);

		MisDestroyableObject target = hit.transform.GetComponent<MisDestroyableObject> ();
		if (target != null)
 			target.DealDamage (firedGun.damage);

		firedGun.DestroyBullet(this);
	}

	public void SetDirection(float dir) {

		_move.x = Mathf.Sign (dir);
	}
}