using UnityEngine;
using System.Collections;

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

	protected override void DidEnterCollision(RaycastHit2D hit) {

		firedGun.DestroyBullet(this);
	}

	public void SetDirection(float dir) {

		_move.x = Mathf.Sign (dir);
	}
}