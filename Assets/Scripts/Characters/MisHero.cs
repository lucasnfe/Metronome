using UnityEngine;
using System.Collections;

public class MisHero : MisCharacter {
	
	public override void Start() {

		base.Start ();
	}

	// Update is called once per frame
	protected override void FixedUpdate () {

		if (!_isDead) {

			KeyboardControl ();
		}

		base.FixedUpdate ();
	}

	private void KeyboardControl() {

		_moveX = 0f;

		if (Input.GetKey (KeyCode.RightArrow)) {
			_moveX = 1f;
			Flip (_moveX);
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			_moveX = -1f;
			Flip (_moveX);
		}

		if (Input.GetKeyDown (KeyCode.UpArrow))
			if(_isOnGround)
				_velocity.y += _jumpSpeed * Time.fixedDeltaTime;

		_isAttacking = Input.GetKey (KeyCode.Space);
	}

	protected override void TriggerEvent(Collider2D collider) {

		if (collider.gameObject.tag == MisConstants.TAG_KILLZONE) {

			MisKillZone killZone = collider.gameObject.GetComponent<MisKillZone> ();
			MisGameWorld.Instance.ResetLevel(killZone.respawnPosition);
		}
	}
}
