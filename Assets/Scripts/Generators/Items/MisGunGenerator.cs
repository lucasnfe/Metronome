using UnityEngine;
using System.Collections;

public static class MisGunGenerator {

	public static MisGun GenerateGun() {

		MisGun gun = new MisGun ();

		// Set a random damage
		gun.damage = Random.Range (1, 10);

		// Set a random frequency
		gun.frequency = Random.Range(0.5f, 2f);

		// Set a random speed
		gun.speed = Random.Range(10f, 20f)/gun.frequency;

		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		gun.texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

		// set the pixel values
		gun.texture.SetPixel(0, 0, Color.white);
		gun.texture.SetPixel(1, 0, Color.white);
		gun.texture.SetPixel(0, 1, Color.white);
		gun.texture.SetPixel(1, 1, Color.white);

		// Apply all SetPixel calls
		gun.texture.Apply();

		return gun;
	}
}
