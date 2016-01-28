using UnityEngine;
using System.Collections;

public class MisHero : MisCharacter {

	private float    _wallImpulse = 4f;
	private float    _shootDelay;
	private MisGun   _gun;
	private Vector2  _wallCollisionNormal;

	protected override void Start() {

		base.Start ();

		// Generate a random gun.
		_gun = MisGunGenerator.GenerateGun();
		_shootDelay = _gun.frequency;
	}

	// Update is called once per frame
	void Update() {

		if (!_isDead)
			KeyboardControl ();
	}

	private void KeyboardControl() {

		// Vertical movement controls
		_move.y = 0f;

		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			if (_isOnGround) {
				
				_move.y = 1f;
			}
			else if (_wallCollisionNormal != Vector2.zero) {
				
				_move.y = 1f;
				_velocity.y = 0f;
			}
		}
			
		// Horizontal movement controls
		_move.x = Input.GetAxisRaw("Horizontal");

		if (!_isOnGround && _move.y == 1f) {

			if( _wallCollisionNormal != Vector2.zero)
				_move.x += _wallImpulse * _wallCollisionNormal.x;
		}

		Flip (_move.x);
	
		// Attacking controls
		_isAttacking = false;

		if (Input.GetKey (KeyCode.Space)) {

			if (_shootDelay >= _gun.frequency) {

				Shoot ();
				_shootDelay = 0f;
				_isAttacking = true;
			}

			_shootDelay += Time.deltaTime * 10f;
		}

		if (Input.GetKeyUp (KeyCode.Space))
			_shootDelay = _gun.frequency;
	}

	void Shoot() {

		float dir = transform.localScale.x;

		Vector3 shootPos = transform.position;
		shootPos.x += (_boundingBox.size.x * 0.5f + 0.05f) * dir;

		_gun.Fire (shootPos, dir);
	}

	protected override void DidEnterEventCollision(RaycastHit2D hit) {

		base.DidEnterEventCollision (hit);

		if (hit.collider.gameObject.tag == MisConstants.TAG_KILLZONE) {

			MisKillZone killZone = hit.collider.gameObject.GetComponent<MisKillZone> ();
			MisGameWorld.Instance.ResetLevel(killZone.respawnPosition);
		}
	}

	protected override void DidEnterCollision(RaycastHit2D hit) {

		base.DidEnterCollision (hit);

		if (hit.collider.tag == "Wall") {
			
			if (hit.normal == Vector2.right || hit.normal == -Vector2.right) 
				_wallCollisionNormal = hit.normal;
			
		}
	}

	protected override void DidExitCollision(RaycastHit2D hit) {
	
		base.DidExitCollision (hit);

		if (hit.collider.tag == "Wall") {

			if (hit.normal == Vector2.right || hit.normal == -Vector2.right) 
				_wallCollisionNormal = Vector2.zero;

		}
	}
}
