using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class MisCamera : MonoBehaviour {

	public MisHero _player;
	private BoxCollider2D _cameraWindow;

	// Use this for initialization
	void Start () {

//		float originalSize = Camera.main.orthographicSize;
//	
//		Camera.main.orthographicSize = Screen.height / (MisConstants.PIXEL_UNIT * 2);
//
		_cameraWindow = GetComponent<BoxCollider2D> ();
//
//		Vector2 adaptedSize = Vector2.zero;
//		adaptedSize.x = (Camera.main.orthographicSize * _cameraWindow.size.x) / originalSize;
//		adaptedSize.y = (Camera.main.orthographicSize * _cameraWindow.size.y) / originalSize;
//
//		_cameraWindow.size = adaptedSize;
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

			if (_player.transform.position.x - _cameraWindow.bounds.min.x >= _cameraWindow.bounds.size.x)
				cameraVelX = heroVel.x;
		}
		else {

			if (_cameraWindow.bounds.max.x -_player.transform.position.x >= _cameraWindow.bounds.size.x)
				cameraVelX = heroVel.x;
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

		transform.position += new Vector3(cameraVelX, cameraVelY, 0f);
	}
}
