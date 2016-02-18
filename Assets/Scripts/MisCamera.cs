using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (Camera))]
public class MisCamera : MonoBehaviour {

	public MisHero _player;

	private BoxCollider2D _cameraWindow;
	private Camera _camera;

	// Use this for initialization
	void Start () {

		_camera = GetComponent<Camera> ();

		_cameraWindow = GetComponent<BoxCollider2D> ();
		_cameraWindow.isTrigger = true;

		_camera.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		float originalSize = _camera.orthographicSize;
		_camera.orthographicSize = Screen.height / (MisConstants.PIXEL_UNIT * 2);

		Vector2 adaptedSize = Vector2.zero;
		adaptedSize.x = (_camera.orthographicSize * _cameraWindow.size.x) / originalSize;
		adaptedSize.y = (_camera.orthographicSize * _cameraWindow.size.y) / originalSize;

		_cameraWindow.size = adaptedSize;
	}

	public void Move(Vector2 dest) {

		transform.position = new Vector3 (dest.x, dest.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

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
