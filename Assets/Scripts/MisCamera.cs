using UnityEngine;
using System.Collections;

public class MisCamera : MonoBehaviour {

	public MisHero _player;

	private BoxCollider2D _heroBoundingBox;
	private BoxCollider2D _cameraWindow;

	private bool _isChasingPlayer;

	// Use this for initialization
	void Start () {
	
		Camera.main.orthographicSize = Screen.height / (MisConstants.PIXEL_UNIT * 2);

		_cameraWindow = GetComponent<BoxCollider2D> ();
	}

	public void Move(Vector2 dest) {

		transform.position = new Vector3 (dest.x, dest.y, transform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if (!_player)
			return;

		Vector2 heroVel = _player.GetDeltaPos ();
	
		// Camera horizontal velocity
		float cameraVelX = 0f;

		if (heroVel.x > 0f) {

			float speed = _cameraWindow.bounds.min.x - _player.transform.position.x;
			cameraVelX = Mathf.Lerp (1.5f, 1f, speed);
		} 

		if (heroVel.x < 0f) {

			float speed = _player.transform.position.x - _cameraWindow.bounds.max.x;
			cameraVelX = Mathf.Lerp (1.5f, 1f, speed);
		}

		// Camera vetical velocity
		float cameraVelY = 0f;
			
		if (heroVel.y > 0f) {

			if (_player.transform.position.y - _cameraWindow.bounds.min.y >= _cameraWindow.bounds.size.y)
				cameraVelY = heroVel.y;
		}
		else {

			if (_cameraWindow.bounds.max.y -_player.transform.position.y >= _cameraWindow.bounds.size.y)
				cameraVelY = heroVel.y;
		}

		transform.position += new Vector3(cameraVelX * heroVel.x, cameraVelY, 0f);
	}
}
