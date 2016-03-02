using UnityEngine;
using System.Collections;

public static class MisGunGenerator {

	public static MisGun GenerateGun(GameObject owner) {

		// Set a random frequency
		float frequency = Random.Range(1f, 2f);

		// Set a random speed
		float speed = 20f/frequency;

		// Set a random damage
		int damage = (int)(10f/frequency);

		int size = Random.Range (4, 8);

		// Create a newtexture
		Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);

		Color []bulletColors = new Color[size * size];
		for(int i = 0; i < bulletColors.Length; i++)
			bulletColors[i] = Color.grey;

		texture.SetPixels (bulletColors);

		// Apply all SetPixel calls
		texture.Apply();

		MisGun gun = new MisGun (damage, speed, frequency, texture, owner);

		return gun;
	}
}
