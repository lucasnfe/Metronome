using UnityEngine;
using System.Collections;

public class MisCamera : MonoBehaviour {

	public GameObject _player;

	// Use this for initialization
	void Start () {
	

	}

	public void Move(Vector2 dest) {

		transform.position = new Vector3 (dest.x, dest.y, transform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if (!_player)
			return;
	
		Bounds camWindow = GetComponent<BoxCollider2D> ().bounds;
		camWindow.center = new Vector3 (camWindow.center.x, camWindow.center.y, _player.transform.position.z);

		Bounds heroBox = _player.GetComponent<BoxCollider2D> ().bounds;
		heroBox.center = new Vector3 (heroBox.center.x, heroBox.center.y, _player.transform.position.z);

		Vector2 heroVel = _player.GetComponent<MisHero> ().GetDeltaPos ();

		// Velocity to move the camera, it will be applied to hero vel
		Vector2 cameraVel = new Vector2 ();

		float dirX = _player.transform.localScale.x;

		// Check horizontal contact
		if (dirX == 1f) {

			float boundX = heroBox.center.x - heroBox.extents.x;
			cameraVel.x = Mathf.Lerp (1.5f, 1f, camWindow.min.x - boundX);
		} else {

			float boundX = heroBox.center.x + heroBox.extents.x;
			cameraVel.x = Mathf.Lerp (1.5f, 1f, boundX - camWindow.max.x);
		}

		transform.position += Vector3.right * (heroVel.x * cameraVel.x);

		float posY = Mathf.Lerp (transform.position.y, _player.transform.position.y, 4f * Time.deltaTime);
		transform.position = new Vector3 (transform.position.x, posY, transform.position.z);
	}
}
