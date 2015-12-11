using UnityEngine;
using System.Collections;

public class MisCamera : MonoBehaviour {

	public GameObject _player;

	// Use this for initialization
	void Start () {
	
		Camera.main.orthographicSize = Screen.height / (MisConstants.PIXEL_UNIT * 2);
	}

	public void Move(Vector2 dest) {

		transform.position = new Vector3 (dest.x, dest.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

		if (!_player)
			return;
	
		Bounds camWindow = GetComponent<BoxCollider2D> ().bounds;
		camWindow.center = new Vector3 (camWindow.center.x, camWindow.center.y, _player.transform.position.z);

		Bounds heroBox = _player.GetComponent<BoxCollider2D> ().bounds;
		heroBox.center = _player.transform.position;

		Vector2 heroVel = _player.GetComponent<MisHero> ().GetDeltaPos ();

		// Velocity to move the camera, it will be applied to hero vel
		Vector2 cameraVel = Vector2.one;

		float dirX = _player.transform.localScale.x;

		// Check horizontal contact
		if (dirX == 1f) {

			float boundX = heroBox.center.x - heroBox.extents.x;
			float speed = camWindow.min.x - boundX > 0f ? camWindow.min.x - boundX : 0f;
			cameraVel.x = Mathf.Lerp (1.5f, 1f, speed);
		} else {

			float boundX = heroBox.center.x + heroBox.extents.x;
			float speed = boundX - camWindow.max.x > 0f ? boundX - camWindow.max.x : 0f;
			cameraVel.x = Mathf.Lerp (1.5f, 1f, speed);
		}

		cameraVel = new Vector2 (heroVel.x * cameraVel.x, heroVel.y * cameraVel.y);
		transform.position += new Vector3 (cameraVel.x, cameraVel.y, 0f);
	}
}
