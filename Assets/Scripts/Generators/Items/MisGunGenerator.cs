using UnityEngine;
using System.Collections;

public static class MisGunGenerator {

	public static MisGun GenerateGun() {

		// Set a random damage
		int damage = Random.Range (1, 10);

		// Set a random frequency
		float frequency = Random.Range(0.5f, 2f);

		// Set a random speed
		float speed = Random.Range(10f, 20f)/frequency;

		int size = Random.Range (2, 6);

		// Create a newtexture
		Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);

		Color []bulletColors = new Color[size * size];
		for(int i = 0; i < bulletColors.Length; i++)
			bulletColors[i] = Color.grey;

		texture.SetPixels (bulletColors);

		// Apply all SetPixel calls
		texture.Apply();

		MisGun gun = new MisGun (damage, speed, frequency, texture);

		return gun;
	}
}
