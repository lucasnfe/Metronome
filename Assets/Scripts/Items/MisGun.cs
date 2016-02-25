using UnityEngine;
using System.Collections;

public class MisGun {

	public int       damage     { get; set; }
	public float     speed      { get; set; }
	public float     frequency  { get; set; }
	public Texture2D texture    { get; set; }

	private MisObjectPool _bullets;
	private GameObject _bulletTemplate;

	public void InitBullet(GameObject obj) {

		MisBullet bullet = obj.GetComponent<MisBullet> ();

		bullet._moveSpeed = speed;
		bullet.firedGun = this;
		bullet.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

		SpriteRenderer bulletSprite = obj.GetComponent<SpriteRenderer> ();
		Rect rect = new Rect (Vector2.zero, new Vector2 (texture.width, texture.height));
		bulletSprite.sprite = Sprite.Create(texture, rect, Vector2.zero);

		bullet.gameObject.AddComponent<BoxCollider2D> ();
	}

	public MisGun(int damage, float speed, float frequency, Texture2D texture) {

		this.damage    = damage;
		this.speed     = speed;
		this.frequency = frequency;
		this.texture   = texture;

		_bulletTemplate = Resources.Load ("Particles/MisBullet") as GameObject;

		_bullets = new MisObjectPool(_bulletTemplate, InitBullet);
	}

	public MisBullet Fire(Vector3 startPos, float dir) {

		GameObject obj = _bullets.GetFreeObject ();
		if (obj) {
			
			MisBullet bullet = obj.GetComponent<MisBullet> ();

			bullet.transform.position = startPos;
			bullet.SetDirection (dir);
			return bullet;
		}

		return null;
	}

	public void DestroyBullet(MisBullet bullet) {

		_bullets.SetFreeObject (bullet.gameObject);
	}
}
