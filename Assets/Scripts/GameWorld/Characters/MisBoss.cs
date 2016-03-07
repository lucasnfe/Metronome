﻿using UnityEngine;
using System.Collections;
using SynchronizerData;

public class MisBoss : MisEnemy {

	public Vector2 FollowingPoint;

	// Use this for initialization
	protected override void Start () {

		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () {

		Vector3 pos = transform.position;
		pos.x = Mathf.Lerp(pos.x, FollowingPoint.x + 3f * MisConstants.TILE_SIZE, _moveSpeed * Time.deltaTime);
		pos.y = Mathf.Lerp(pos.y, FollowingPoint.y + 1f * MisConstants.TILE_SIZE, _jumpSpeed * Time.deltaTime);
		transform.position = pos;
	}
}