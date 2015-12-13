using UnityEngine;
using System.Collections;

public class MisHero : MisCharacter {

	private MisGun _gun;
	private float  _shootDelay;
	private bool   _isAtWall;

	protected override void Start() {

		base.Start ();

		// Generate a random gun.
		_gun = MisGunGenerator.GenerateGun();
		_shootDelay = _gun.frequency;
	}

	// Update is called once per frame
	protected override void FixedUpdate () {

		if (!_isDead) {

			KeyboardControl ();
		}

		base.FixedUpdate ();
	}

	private void KeyboardControl() {

		// Vertical moviment
		_move.y = 0f;

		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			if(_isOnGround)
				_move.y = 1f;

			if (_isAtWall) {
				_velocity.y = 0f;
				_move.y = 1f;
			}
		}

		_isAtWall = false;

		// Horizontal moviment
		_move.x = Input.GetAxisRaw ("Horizontal");
		if (_move.x != 0f)
			Flip (_move.x);
			
		// Attacking
		_isAttacking = false;

		if (Input.GetKey (KeyCode.Space)) {

			if (_shootDelay >= _gun.frequency) {

				Shoot ();
				_shootDelay = 0f;
				_isAttacking = true;
			}

			_shootDelay += Time.fixedDeltaTime * 100f;
		}

		if (Input.GetKeyUp (KeyCode.Space)) {

			_shootDelay = _gun.frequency;
		}
	}

	void Shoot() {

		float dir = transform.localScale.x;

		Vector3 shootPos = transform.position;
		shootPos.x += (_boundingBox.size.x * 0.5f + 0.05f) * dir;

		_gun.Fire (shootPos, dir);
	}

	protected override void DidEnterEventCollision(RaycastHit2D hit) {

		if (hit.collider.gameObject.tag == MisConstants.TAG_KILLZONE) {

			MisKillZone killZone = hit.collider.gameObject.GetComponent<MisKillZone> ();
			MisGameWorld.Instance.ResetLevel(killZone.respawnPosition);
		}
	}

	protected override void DidEnterCollision(RaycastHit2D hit) {

		if (hit.collider.tag == "Wall") {
			if (hit.normal == Vector2.right || hit.normal == -Vector2.right) {
				_isAtWall = true;
			}
		}
	}
}
