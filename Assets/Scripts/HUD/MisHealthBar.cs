using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Slider))]
public class MisHealthBar : MonoBehaviour {

	private Slider _healthSlider;

	private float  _currentHealth;
	public  float  _startHealth = 1f;

	// Use this for initialization
	void Start () {

		_currentHealth = _startHealth;

		_healthSlider = GetComponent<Slider> ();
		SetHealthBar (_currentHealth);
	}

	public void SetHealthBar(float value) {

		value = Mathf.Clamp (value, _healthSlider.minValue, _healthSlider.maxValue);
		_currentHealth = value;
		_healthSlider.value = _currentHealth;
	}
}