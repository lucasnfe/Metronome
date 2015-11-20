using UnityEngine;
using System.Collections;

public class MisHero : MisCharacter {
	
	public override void Start() {

		base.Start ();
	}

	// Update is called once per frame
	public override void Update () {

		if (!_isDead) {

			// Detect player input
			KeyboardControl ();
		}

		// Update character physics
		base.Update ();		
	}

	private void KeyboardControl() {
	
		_moveX = 0f;

		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow)) {

			_moveX = Input.GetAxis("Horizontal");
			transform.localScale = new Vector3 (Mathf.Sign(_moveX), 1f, 1f);
		}

		if (_isOnGround && Input.GetKeyDown (KeyCode.UpArrow))
			_velocity.y = _jumpSpeed;

		_isAttacking = Input.GetKey (KeyCode.Space);
	}

	protected override void TriggerEvent(Collider2D collider) {

		if (collider.gameObject.tag == MisConstants.TAG_KILLZONE) {

			MisKillZone killZone = collider.gameObject.GetComponent<MisKillZone> ();
			MisGameWorld.Instance.ResetLevel(killZone.respawnPosition);
		}
	}
}
