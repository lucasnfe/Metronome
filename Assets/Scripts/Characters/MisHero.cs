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

		_isAttacking = Input.GetKey (KeyCode.Space);

		if(Input.GetKey (KeyCode.RightArrow)) {

			transform.localScale = new Vector3(1f, 1f, 1f);
			_moveX = 1f;
		}

		if(Input.GetKey (KeyCode.LeftArrow)) {

			transform.localScale = new Vector3(-1f, 1f, 1f);
			_moveX = -1f;
		}

		if (_isOnGround && Input.GetKeyDown( KeyCode.UpArrow)) {

			_velocity.y += _jumpSpeed;
		}	
	}

	protected override void TriggerEvent(Collider2D collider) {

		if (collider.gameObject.tag == MisConstants.TAG_KILLZONE) {

			MisKillZone killZone = collider.gameObject.GetComponent<MisKillZone> ();
			MisGameWorld.Instance.ResetLevel(killZone.respawnPosition);
		}
	}
}
