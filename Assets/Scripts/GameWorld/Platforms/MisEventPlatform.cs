using UnityEngine;
using System.Collections;

public class MisEventPlatform : MonoBehaviour {

	public delegate void PlatformEvent();

	private int _eventCounter = 0;

	public int EventLimit      { get; set; }
	public Vector2 Normal      { get; set; }
	public PlatformEvent Event { get; set; }

	void Start() {

		EventLimit = int.MaxValue;
	}

	public void ExecEvent(Vector2 collisionNormal) {

		_eventCounter++; 

		if (collisionNormal == Normal && Event != null)
			Event ();

		if (_eventCounter >= EventLimit)
			Destroy (gameObject);
	}
}
