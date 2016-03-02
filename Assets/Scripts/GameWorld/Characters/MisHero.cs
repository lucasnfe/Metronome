using UnityEngine;
using System.Collections;

public class MisHero : MisCharacter {

	private float    _shootDelay;
	private float    _jumpTimer;
	private float    _stepTimer;

	private MisGun   _gun;
	private Vector2  _wallCollisionNormal;

	public  float    _longJumpTime;
	public  float    _stepDelay;

	protected override void Start() {

		base.Start ();

		// Generate a random gun.
		_gun = MisGunGenerator.GenerateGun(gameObject);
		_shootDelay = _gun.frequency;
		_stepTimer  = _stepDelay;
	}

	// Update is called once per frame
	protected override void Update() {

		base.Update ();

		if (!_isDead)
			KeyboardControl ();
	}

	private void KeyboardControl() {

		_renderer.flipX = IsGliding();

		VerticalMovement();
			
		HorizontalMovement();

		AttackingBehaviour();
	}

	void VerticalMovement() {

		bool canJump = _isOnGround;
		if (_wallCollisionNormal != Vector2.zero)
			canJump = true;

		if (canJump && Input.GetKeyDown(KeyCode.UpArrow)){

			_jumpTimer = _longJumpTime;

			ApplyForce (Vector2.up * _jumpSpeed * Time.fixedDeltaTime);

			if (!_isOnGround && _wallCollisionNormal != Vector2.zero) {

				_velocity.y = 0f;
				ApplyForce (Vector2.right * _moveSpeed * _wallCollisionNormal.x * 5f * Time.fixedDeltaTime);
			}

			PlaySFX ((int)CHARACTER_SFX.JUMP);
		}

		if (_jumpTimer > 0f) {
			
			if (Input.GetKeyUp(KeyCode.UpArrow)) {

				_jumpTimer = 0f;

			} 
			else if(Input.GetKey(KeyCode.UpArrow)) {
				
				ApplyForce (Vector2.up * _jumpSpeed * _jumpTimer * Time.fixedDeltaTime);

				_jumpTimer -= Time.fixedDeltaTime;
				if (_jumpTimer <= 0f) {
					_jumpTimer = 0f;
				}
			}
		}
	}

	void HorizontalMovement() {

		float dir = Input.GetAxisRaw("Horizontal");
		ApplyForce (Vector2.right * _moveSpeed * dir * Time.fixedDeltaTime);

		if (_isOnGround) {

			if (Mathf.Abs (dir) > 0f) {

				_stepTimer += Time.deltaTime;
				if (_stepTimer >= _stepDelay) {

					PlaySFX ((int)CHARACTER_SFX.RUN);
					_stepTimer = 0f;
				}
			} 
			else
				_stepTimer = _stepDelay;
		}
	}

	void AttackingBehaviour() {

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

		if (IsGliding())
			dir = _wallCollisionNormal.x;

		Vector3 shootPos = transform.position;
		shootPos.x += (_boundingBox.size.x * 0.5f + 0.05f) * dir;

		PlaySFX ((int)CHARACTER_SFX.SHOOT);

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

	protected override void DidStayCollision(Collider2D hit, Vector2 normal) {

		if (hit == null)
			return;

		base.DidStayCollision (hit, normal);

		if (hit.tag == "Wall") {

			if (normal == Vector2.right || normal == -Vector2.right) 
				_wallCollisionNormal = normal;
		}
	}

	protected override void DidExitCollision(Collider2D hit, Vector2 normal) {
	
		if (hit == null) {
			_wallCollisionNormal = Vector2.zero;
			return;
		}

		base.DidExitCollision (hit, normal);

		if (hit.tag == "Wall") {
				_wallCollisionNormal = Vector2.zero;

		}
	}

	protected bool IsGliding() {

		bool isPressingRun = Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow);
		return !_isOnGround && _wallCollisionNormal != Vector2.zero && _velocity.y < 0f && isPressingRun;
	}
}
