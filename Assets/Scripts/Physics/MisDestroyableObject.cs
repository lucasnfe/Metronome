using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class MisDestroyableObject : MonoBehaviour {

	private Animator _animator;

	public int _life = 10;
	public int Life { get { return _life; } }

	void Start() {

		_animator = GetComponent<Animator> ();
	}

	public void DealDamage(int damage) {

		_life -= damage;

		_animator.SetTrigger ("hit");

		if (_life <= 0)
			Destroy (gameObject);
	}
}
