using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (Camera))]
public class MisCamera : MonoBehaviour {

	private Vector3       _move;
	private MisHero       _player;
	private Camera        _camera;
	private BoxCollider2D _cameraWindow;

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

		_player = MisGameWorld.Instance.GameHero;

		if (!_player)
			return;

		Vector2 heroVel = _player.Velocity;
		_move = Vector2.zero;
	
		// Camera horizontal velocity
		if (heroVel.x > 0f) {

			if (_player.transform.position.x - _cameraWindow.bounds.min.x >= _cameraWindow.bounds.size.x)
				_move.x = heroVel.x;
		}
		else {

			if (_cameraWindow.bounds.max.x -_player.transform.position.x >= _cameraWindow.bounds.size.x)
				_move.x = heroVel.x;
		}

		// Camera vetical velocity			
		if (heroVel.y > 0f) {

			if (_player.transform.position.y - _cameraWindow.bounds.min.y >= _cameraWindow.bounds.size.y)
					_move.y = heroVel.y;
		}
		else {

			if (_cameraWindow.bounds.max.y -_player.transform.position.y >= _cameraWindow.bounds.size.y)
					_move.y = heroVel.y;
		}
			
		transform.position += _move;
	}
}
