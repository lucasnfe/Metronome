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
	protected override void Update() {

		base.Update ();

		if (!_isDead)
			KeyboardControl ();
	}

	private void KeyboardControl() {

		// Vertical movement controls
		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			if (_isOnGround) {

				// Ground jumping
				_move.y = 1f;
			} 
			else if (_wallCollisionNormal != Vector2.zero) {

				// Wall jumping
				_move.y = 1f;
				_velocity.y = 0f;
			}
		}
			
		// Horizontal movement controls
		_move.x = Input.GetAxisRaw("Horizontal");

		if (!_isOnGround && _move.y > 0f) {

			// Wall jumping
			if(_wallCollisionNormal != Vector2.zero)
				_move.x += _wallImpulse * _wallCollisionNormal.x;
		}

		Flip (_move.x);
	
		// Attacking controls
		_isAttacking = false;

		if (Input.GetKeyDown (KeyCode.Space)) {

			if (_shootDelay >= _gun.frequency) {

				Shoot ();
				_shootDelay = 0f;
				_isAttacking = true;
			}

			_shootDelay += Time.deltaTime;
		}

		if (Input.GetKeyUp (KeyCode.Space))
			_shootDelay = _gun.frequency;
	}

	void Shoot() {

		float dir = transform.localScale.x;

		if(!_isOnGround && _wallCollisionNormal != Vector2.zero)
			dir = _wallCollisionNormal.x;

		Vector3 shootPos = transform.position;
		shootPos.x += (_boundingBox.size.x * 0.5f + 0.05f) * dir;

		_gun.Fire (shootPos, dir);
	}

	protected override void DidEnterEventCollision(Collider2D hit, Vector2 normal) {

		if (hit == null)
			return;
		
		base.DidEnterEventCollision (hit, normal);

		if (hit.gameObject.tag == MisConstants.TAG_KILLZONE) {

			MisKillZone killZone = hit.transform.gameObject.GetComponent<MisKillZone> ();
			MisGameWorld.Instance.ResetLevel(killZone.respawnPosition);
		}
	}

	protected override void DidEnterCollision(Collider2D hit, Vector2 normal) {

		if (hit == null)
			return;
		
		base.DidEnterCollision (hit, normal);

		if (hit.tag == "Wall") {
			
			if (normal == Vector2.right || normal == -Vector2.right) 
				_wallCollisionNormal = normal;
			
		}
	}

	protected override void DidExitCollision(Collider2D hit, Vector2 normal) {
	
		if (hit == null)
			return;

		base.DidExitCollision (hit, normal);

		if (hit.tag == "Wall") {

			if (normal == Vector2.right || normal == -Vector2.right) 
				_wallCollisionNormal = Vector2.zero;

		}
	}
}
