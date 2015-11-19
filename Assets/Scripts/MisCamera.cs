using UnityEngine;
using System.Collections;

public class MisCamera : MonoBehaviour {

	public GameObject _player;

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {

		if (!_player)
			return;
	
		Bounds camWindow = GetComponent<BoxCollider2D> ().bounds;
		camWindow.center = new Vector3 (camWindow.center.x, camWindow.center.y, _player.transform.position.z);

		Bounds heroBox = _player.GetComponent<BoxCollider2D> ().bounds;
		heroBox.center = new Vector3 (heroBox.center.x, heroBox.center.y, _player.transform.position.z);

		Vector2 heroVel = _player.GetComponent<MisHero> ().GetDeltaPos ();

		// Velocity to move the camera, it will be applied to hero vel
		Vector2 cameraVel = new Vector2 ();
	
		if (heroVel.x != 0f) {

			float dirX = _player.transform.localScale.x;

			// Check horizontal contact
			if (dirX == 1f) {

				float boundX = heroBox.center.x - heroBox.extents.x;
				cameraVel.x = Mathf.Lerp (1.5f, 1f, camWindow.min.x - boundX);
			} else {

				float boundX = heroBox.center.x + heroBox.extents.x;
				cameraVel.x = Mathf.Lerp (1.5f, 1f, boundX - camWindow.max.x);
			}
		}

		if (heroVel.y != 0f) {

			float dirY = Mathf.Sign(heroVel.y);

			// Check virtual contact
			if (dirY == 1f) {

				float boundY = heroBox.center.y + heroBox.extents.y;
				if (camWindow.max.y < boundY) {
		
					cameraVel.y = 1.2f;
				}
			} else {

				float boundY = heroBox.center.y - heroBox.extents.y;
				if (camWindow.min.y > boundY) {
						
					cameraVel.y = 1.2f;
				}
			}
		}

		transform.position += new Vector3(heroVel.x * cameraVel.x, heroVel.y * cameraVel.y, 0f);
	}
}
