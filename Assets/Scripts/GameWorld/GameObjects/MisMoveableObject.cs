using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisMoveableObject : MisDestroyableObject {

	protected Vector2  _velocity;
	public Vector2 Velocity {  get { return  _velocity; } set { _velocity = value; }  }

	protected Vector2  _acceleration;
	public Vector2 Acceleration { get { return _acceleration; } }

	protected bool  _isOnGround;

	protected Vector2  _ray;

	// Moveable object configurations
	public int   _raysAmount;
	public float _mass;
	public float _moveSpeed;
	public float _jumpSpeed;
	public float _gravityScale = 1f;
	public bool  _applyGravity;
	public bool  _detectHorCollision;
	public bool  _detectVerCollision;

	private RaycastHit2D   _rayHit;
	private RaycastHit2D   _rayMissHit;
	private RaycastHit2D []_rayHitTrigger;

	private Dictionary<Vector2, RaycastHit2D> _vertCollisions;
	private Dictionary<Vector2, RaycastHit2D> _horiCollisions;

	private Dictionary<Vector2, RaycastHit2D> _vertEventCollisions;
	private Dictionary<Vector2, RaycastHit2D> _horiEventCollisions;

	protected override void Start() {

		base.Start ();

		_boundingBox = GetComponent<BoxCollider2D> ();

		_vertCollisions = new Dictionary<Vector2, RaycastHit2D> ();
		_horiCollisions = new Dictionary<Vector2, RaycastHit2D> ();

		_vertEventCollisions = new Dictionary<Vector2, RaycastHit2D> ();
		_horiEventCollisions = new Dictionary<Vector2, RaycastHit2D> ();

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
			ApplyForce (Physics2D.gravity * _gravityScale * Time.fixedDeltaTime);

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

		if(_detectVerCollision)
			DetectVerticalCollision (entityPosition, offset, size);

		if (_detectHorCollision) {

			if (DetectHorizontalCollision (entityPosition, offset, size)) {

				if (!_isOnGround && _velocity.y < 0f) {
					
					// Apply wall friction					
					float tv = Vector2.Dot (_velocity, Vector2.up) * MisConstants.WALL_FRICTION;
					_velocity -= Vector2.up * tv;
				}
			}
		}

		DetectVerticalTriggerCollision(entityPosition, offset, size);

		DetectHorizontalTriggerCollision (entityPosition, offset, size);
	}

	private bool DetectHorizontalCollision(Vector2 entityPosition, Vector2 offset, Vector2 size) {

		for (int rayPos = 0; rayPos < _raysAmount; rayPos++) {

			_rayHit = xAxisRaycasts (entityPosition, offset, size, rayPos);
			if (_rayHit.collider)
				break;
		}

		if (_rayHit.collider == null) {

			foreach (RaycastHit2D hit in _horiCollisions.Values)
				DidExitCollision (hit.collider, hit.normal);

			return false;
		}

		return true;
	}

	private RaycastHit2D xAxisRaycasts(Vector2 entityPosition, Vector2 offset, Vector2 size, int i) {

		float dirX = transform.localScale.x;

		_ray.x = entityPosition.x + (offset.x + size.x / 2f + MisConstants.PLAYER_SKIN) * dirX;
		_ray.y = (entityPosition.y + offset.y - size.y / 2f + MisConstants.PLAYER_SKIN) + (size.y / 2f) * i;

		RaycastHit2D[] hits = Physics2D.RaycastAll (_ray, Vector2.right * dirX, Mathf.Abs (_velocity.x));
		Debug.DrawRay (_ray, new Vector2 (dirX, 0));

		foreach (RaycastHit2D hit in hits) {

			if (hit.collider == null) {
				_rayMissHit = hit;
				continue;
			}

			if (hit.collider.isTrigger)
				continue;

			float distance = Mathf.Abs (_ray.x - hit.point.x);

			if (distance >= MisConstants.PLAYER_SKIN)
				_velocity.x = (distance - MisConstants.PLAYER_SKIN) * dirX;
			else
				_velocity.x = 0f;

			if (!_horiCollisions.ContainsKey(hit.transform.position)) {

				_horiCollisions [hit.transform.position] = hit;
				DidEnterCollision (hit.collider, hit.normal);
			} 
			else
				DidStayCollision (_horiCollisions [hit.transform.position].collider,
					_horiCollisions [hit.transform.position].normal);

			return hit;
		}

		return _rayMissHit;
	}

	private void DetectHorizontalTriggerCollision(Vector2 entityPosition, Vector2 offset, Vector2 size) {

		for (int rayPos = 0; rayPos < _raysAmount; rayPos++)
			_rayHitTrigger = xAxisTriggerRaycasts (entityPosition, offset, size, rayPos);

		if (_rayHitTrigger.Length == 0) {

			foreach (RaycastHit2D hit in _horiEventCollisions.Values)
				DidExitEventCollision (hit.collider, hit.normal);

			_vertEventCollisions.Clear ();
		}
	}

	private RaycastHit2D[] xAxisTriggerRaycasts(Vector2 entityPosition, Vector2 offset, Vector2 size, int i) {

		float dirX = transform.localScale.x;

		_ray.x = entityPosition.x + (offset.x + size.x / 2f + MisConstants.PLAYER_SKIN) * dirX;
		_ray.y = (entityPosition.y + offset.y - size.y / 2f + MisConstants.PLAYER_SKIN) + (size.y / 2f) * (float)i;

		RaycastHit2D[] hits = Physics2D.RaycastAll (_ray, Vector2.right * dirX, Mathf.Abs (_velocity.x));
		//		Debug.DrawRay (_ray, new Vector2 (dirX, 0));

		foreach (RaycastHit2D hit in hits) {

			if (hit.collider == null)
				continue;

			if (hit.collider.isTrigger) {

				if (!_horiEventCollisions.ContainsKey (hit.transform.position)) {

					_horiEventCollisions [hit.transform.position] = hit;
					DidEnterEventCollision (hit.collider, hit.normal);
				}
			}
		}

		return hits;
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

		return true;
	}

	private RaycastHit2D yAxisRaycasts(Vector2 entityPosition, Vector2 offset, Vector2 size, int i) {

		float dirX = -transform.localScale.x;
		float dirY =  Mathf.Sign(_velocity.y);

		_ray.x = (entityPosition.x + offset.x * -dirX - size.x / 2f) + size.x / 2f * i;
		_ray.y = entityPosition.y + offset.y + size.y / 2f * dirY + MisConstants.PLAYER_SKIN * dirY;

		RaycastHit2D[] hits = Physics2D.RaycastAll(_ray, Vector2.up * dirY, Mathf.Abs(_velocity.y));
//		Debug.DrawRay(_ray,  Vector2.up * dirY);
	
		foreach (RaycastHit2D hit in hits) {

			if (hit.collider == null) {
				_rayMissHit = hit;
				continue;
			}

			if (hit.collider.isTrigger)
				continue;

			float distance = Mathf.Abs (_ray.y - hit.point.y);

			if (distance >= MisConstants.PLAYER_SKIN)
				_velocity.y = (distance - MisConstants.PLAYER_SKIN) * dirY;
			else
				_velocity.y = 0f;

			if (!_vertCollisions.ContainsKey(hit.transform.position)) {

				_vertCollisions [hit.transform.position] = hit;
				DidEnterCollision (hit.collider, hit.normal);
			} 
			else
				DidStayCollision (_vertCollisions [hit.transform.position].collider, 
					_vertCollisions [hit.transform.position].normal);

			return hit;
		}

		return _rayMissHit;
	}

	private void DetectVerticalTriggerCollision(Vector2 entityPosition, Vector2 offset, Vector2 size) {

		if (transform.localScale.x == 1f) {

			for (int rayPos = _raysAmount - 1; rayPos >= 0; rayPos--) {

				_rayHitTrigger = yAxisTriggerRaycasts (entityPosition, offset, size, rayPos);
			}
		} 
		else {

			for (int rayPos = 0; rayPos < _raysAmount; rayPos++) {

				_rayHitTrigger = yAxisTriggerRaycasts (entityPosition, offset, size, rayPos);
			}
		}

		if (_rayHitTrigger.Length == 0) {

			foreach (RaycastHit2D hit in _vertEventCollisions.Values)
				DidExitEventCollision (hit.collider, hit.normal);

			_vertEventCollisions.Clear ();
		}
	}

	private RaycastHit2D[] yAxisTriggerRaycasts(Vector2 entityPosition, Vector2 offset, Vector2 size, int i) {

		float dirX = -transform.localScale.x;
		float dirY =  Mathf.Sign(_velocity.y);

		_ray.x = (entityPosition.x + offset.x * -dirX - size.x / 2f) + size.x / 2f * i;
		_ray.y = entityPosition.y + offset.y + size.y / 2f * dirY + MisConstants.PLAYER_SKIN * dirY;

		RaycastHit2D[] hits = Physics2D.RaycastAll(_ray, Vector2.up * dirY, Mathf.Abs(_velocity.y));
		//		Debug.DrawRay(_ray,  Vector2.up * dirY);

		foreach (RaycastHit2D hit in hits) {

			if (hit.collider == null)
				continue;

			if (hit.collider.isTrigger) {

				if (!_vertEventCollisions.ContainsKey (hit.transform.position)) {

					_vertEventCollisions [hit.transform.position] = hit;
					DidEnterEventCollision (hit.collider, hit.normal);
				}
			}
		}

		return hits;
	}
		
	protected virtual void DidEnterEventCollision(Collider2D hit, Vector2 normal) {

	}

	protected virtual void DidExitEventCollision(Collider2D hit, Vector2 normal) {

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