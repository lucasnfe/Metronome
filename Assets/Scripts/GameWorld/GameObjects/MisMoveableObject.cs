﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisMoveableObject : MisDestroyableObject {

	protected Vector2  _velocity;
	public Vector2 Velocity { 
		get { return _velocity; } 
		set { _velocity = value; } 
	}

	protected Vector2  _acceleration;
	public Vector2 Acceleration { get { return _acceleration; } }

	protected Vector2  _ray;

	// States
	protected bool _isOnGround;

	// Moveable object configurations
	public int   _raysAmount;
	public float _mass;
	public float _moveSpeed;
	public float _jumpSpeed;
	public bool  _applyGravity;
	public bool  _detectHorCollision;
	public bool  _detectVerCollision;

	private RaycastHit2D _rayHit;

	private Dictionary<Vector2, RaycastHit2D> _vertCollisions;
	private Dictionary<Vector2, RaycastHit2D> _horicollisions;

	protected override void Start() {

		base.Start ();

		_boundingBox = GetComponent<BoxCollider2D> ();

		_vertCollisions = new Dictionary<Vector2, RaycastHit2D> ();
		_horicollisions = new Dictionary<Vector2, RaycastHit2D> ();

		_rayHit = new RaycastHit2D ();
	}

	// Update is called once per frame
	protected virtual void FixedUpdate () {

		// Add gravity and friction
		CalculateVelocity ();

		// Detect collisiong and modify velocity vector
		DetectCollision ();

		// Detect collision and integrate velocity to position
		transform.Translate(_velocity);
	}
		
	private void CalculateVelocity() {

		// Apply gravity acceleration
		if (_applyGravity)
			ApplyForce (Vector2.up * Physics2D.gravity.y * Time.fixedDeltaTime);

		// Update velocity using currently acceleration
		_velocity += _acceleration;

		// Apply air friction
		float gv = Vector2.Dot (_velocity, Vector2.right) * MisConstants.AIR_FRICTION;
		_velocity -= Vector2.right * gv;

		// Reset acceleration
		_acceleration = Vector2.zero;

		_velocity.x = Mathf.Clamp (_velocity.x, -MisConstants.MAX_SPEED, MisConstants.MAX_SPEED);
		_velocity.y = Mathf.Clamp (_velocity.y, -MisConstants.MAX_SPEED, MisConstants.MAX_SPEED);

		Flip (_velocity.x);
	}

	public void ApplyForce(Vector2 force) {

		if(_mass != 0f)
			force /= _mass;
		
		_acceleration += force;
	}

	private void DetectCollision() {
		
		Vector2 size = _boundingBox.bounds.size;
		Vector2 offset = _boundingBox.offset;
		Vector2 entityPosition = _boundingBox.bounds.center;

		DetectVerticalCollision (entityPosition, offset, size);

		if (Mathf.Abs (_velocity.x) > MisConstants.SAFETY_GAP) {

			if (DetectHorizontalCollision (entityPosition, offset, size)) {

				if (!_isOnGround && _velocity.y < 0f) {
					
					// Apply wall friction					
					float tv = Vector2.Dot (_velocity, Vector2.up) * MisConstants.WALL_FRICTION;
					_velocity -= Vector2.up * tv;
				}
			}
		}
	}

	private bool DetectHorizontalCollision(Vector2 entityPosition, Vector2 offset, Vector2 size) {
								
		for (int rayPos = 0; rayPos < _raysAmount; rayPos++) {

			_rayHit = xAxisRaycasts (entityPosition, offset, size, rayPos);
			if (_rayHit.collider)
				break;
		}

		if (_rayHit.collider == null) {

			foreach (RaycastHit2D hit in _horicollisions.Values)
				DidExitCollision (hit.collider, hit.normal);

			_horicollisions.Clear ();
			return false;
		}	

		if (!_detectHorCollision)
			return false;

		return true;
	}

	private RaycastHit2D xAxisRaycasts(Vector2 entityPosition, Vector2 offset, Vector2 size, int i) {

		float dirX = transform.localScale.x;
		float yRayOffset = (size.y / 2f - MisConstants.SAFETY_GAP) * (float)i;

		_ray.x = entityPosition.x + (offset.x + size.x / 2f + MisConstants.PLAYER_SKIN) * dirX;
		_ray.y = (entityPosition.y + offset.y - size.y / 2f + MisConstants.PLAYER_SKIN) + yRayOffset;

		RaycastHit2D hit = Physics2D.Raycast (_ray, Vector2.right * dirX, Mathf.Abs (_velocity.x));
//		Debug.DrawRay (_ray, new Vector2 (dirX, 0));

		if (hit.collider) {

			if (!hit.collider.isTrigger) {

				float distance = Mathf.Abs (_ray.x - hit.point.x);

				if (_detectHorCollision) {

					if (distance >= MisConstants.PLAYER_SKIN)
						_velocity.x = (distance - MisConstants.PLAYER_SKIN) * dirX;
					else
						_velocity.x = 0f;
				}

				if (!_horicollisions.ContainsKey(hit.transform.position)) {

					_horicollisions [hit.transform.position] = hit;
					DidEnterCollision (hit.collider, hit.normal);
				} 
				else
					DidStayCollision (_horicollisions [hit.transform.position].collider,
						_horicollisions [hit.transform.position].normal);

				return hit;
			}

			DidEnterEventCollision (hit.collider, hit.normal);
		}

		return hit;
	}
		
	private bool DetectVerticalCollision(Vector2 entityPosition, Vector2 offset, Vector2 size) {

		if (transform.localScale.x == 1f) {

			for (int rayPos = _raysAmount - 1; rayPos >= 0; rayPos--) {

				_rayHit = yAxisRaycasts (entityPosition, offset, size, rayPos);
				if (_rayHit.collider)
					break;
			}
		} 
		else {
			
			for (int rayPos = 0; rayPos < _raysAmount; rayPos++) {

				_rayHit = yAxisRaycasts (entityPosition, offset, size, rayPos);
				if (_rayHit.collider)
					break;
			}
		}
			
		if (_rayHit.collider == null) {

			foreach (RaycastHit2D hit in _vertCollisions.Values)
				DidExitCollision (hit.collider, hit.normal);

			_vertCollisions.Clear ();
			return false;
		}

		if (!_detectVerCollision)
			return false;

		return true;
	}

	private RaycastHit2D yAxisRaycasts(Vector2 entityPosition, Vector2 offset, Vector2 size, int i) {

		float dirX = -transform.localScale.x;
		float dirY =  Mathf.Sign(_velocity.y);

		_ray.x = (entityPosition.x + offset.x * -dirX - size.x / 2f) + size.x / 2f * i;
		_ray.y = entityPosition.y + offset.y + size.y / 2f * dirY + MisConstants.PLAYER_SKIN * dirY;

		RaycastHit2D hit = Physics2D.Raycast(_ray, Vector2.up * dirY, Mathf.Abs(_velocity.y));
//		Debug.DrawRay(_ray,  Vector2.up * dirY);

		if (hit.collider) {

			if (!hit.collider.isTrigger) {

				float distance = Mathf.Abs (_ray.y - hit.point.y);

				if (_detectVerCollision) {

					if (distance >= MisConstants.PLAYER_SKIN)
						_velocity.y = (distance - MisConstants.PLAYER_SKIN) * dirY;
					else
						_velocity.y = 0f;
				}

				if (!_vertCollisions.ContainsKey(hit.transform.position)) {

					_vertCollisions [hit.transform.position] = hit;
					DidEnterCollision (hit.collider, hit.normal);
				} 
				else
					DidStayCollision (_vertCollisions [hit.transform.position].collider, 
						_vertCollisions [hit.transform.position].normal);

				return hit;
			}

			DidEnterEventCollision (hit.collider, hit.normal);
		}

		return hit;
	}
		
	protected virtual void DidEnterEventCollision(Collider2D hit, Vector2 normal) {

	}

	protected virtual void DidEnterCollision(Collider2D hit, Vector2 normal) {

		if (hit == null)
			return;

		if (normal == Vector2.up)
			_isOnGround = true;
	}

	protected virtual void DidStayCollision(Collider2D hit, Vector2 normal) {

	}

	protected virtual void DidExitCollision(Collider2D hit, Vector2 normal) {

		if (hit == null)
			return;

		if (normal == Vector2.up)
			_isOnGround = false;
	}

	protected void Flip(float dir) {

		if (dir != 0f) {
				
			Vector3 temp = transform.localScale;
			temp.x = Mathf.Sign (dir);
			transform.localScale = temp;
		}
	}

	public bool IsFlipped() {

		return transform.localScale.x == -1f;
	}

	public bool IsJumping() {

		return _velocity.y > 0f;
	}

	public bool IsFalling() {

		return _velocity.y < 0f && !_isOnGround;
	}

	public bool IsRunning() {

		return _velocity.x != 0f;
	}
}