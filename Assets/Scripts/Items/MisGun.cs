using UnityEngine;
using System.Collections;

public class MisGun {

	public int       damage     { get; set; }
	public float     speed      { get; set; }
	public float     frequency  { get; set; }
	public Texture2D texture    { get; set; }

	private MisObjectPool _bullets;
	private GameObject _bulletTemplate;

	public MisGun() { 

		_bulletTemplate = Resources.Load ("Particles/MisBullet") as GameObject;
		_bullets = new MisObjectPool(_bulletTemplate);
	}

	public MisGun(int damage, float speed, float frequency, Texture2D texture) {

		this.damage    = damage;
		this.speed     = speed;
		this.frequency = frequency;
		this.texture   = texture;
	}

	public MisBullet Fire(Vector3 startPos, float dir) {

		GameObject obj = _bullets.GetFreeObject ();
		MisBullet bullet = obj.GetComponent<MisBullet> ();

		bullet._moveSpeed = speed;
		bullet.firedGun = this;
		bullet.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
		bullet.transform.position = startPos;

		SpriteRenderer bulletSprite = obj.GetComponent<SpriteRenderer> ();
		Rect rect = new Rect (Vector2.zero, new Vector2 (2f, 2f));
		bulletSprite.sprite = Sprite.Create(texture, rect, Vector2.zero);

		bullet.gameObject.AddComponent<BoxCollider2D> ();
		bullet.SetDirection (dir);

		return bullet;
	}

	public void DestroyBullet(MisBullet bullet) {

		_bullets.SetFreeObject (bullet.gameObject);
	}
}
