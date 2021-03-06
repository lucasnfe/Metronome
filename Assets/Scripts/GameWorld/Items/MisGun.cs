﻿using UnityEngine;
using System.Collections;

public class MisGun {

	public int        damage     { get; set; }
	public float      speed      { get; set; }
	public float      frequency  { get; set; }
	public Texture2D  texture    { get; set; }
	public GameObject owner      { get; set; }

	private MisObjectPool _bullets;
	private GameObject _bulletTemplate;

	public void InitBullet(GameObject obj) {

		MisBullet bullet = obj.GetComponent<MisBullet> ();

		bullet._moveSpeed = speed;
		bullet.firedGun = this;
		bullet.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");

//		SpriteRenderer bulletSprite = obj.GetComponent<SpriteRenderer> ();
//		if (bulletSprite.sprite == null) {
//			Rect rect = new Rect (Vector2.zero, new Vector2 (texture.width, texture.height));
//			bulletSprite.sprite = Sprite.Create (texture, rect, Vector2.zero);
//		}

//		bullet.gameObject.AddComponent<BoxCollider2D> ();
	}

	public MisGun(int damage, float speed, float frequency, Texture2D texture, GameObject owner) {

		this.damage    = damage;
		this.speed     = speed;
		this.frequency = frequency;
		this.texture   = texture;
		this.owner     = owner;

		_bulletTemplate = Resources.Load ("Prefabs/Particles/MisBullet") as GameObject;

		this.speed = _bulletTemplate.GetComponent<MisBullet>()._moveSpeed;
		_bullets = new MisObjectPool(_bulletTemplate, 50, null, InitBullet);
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
