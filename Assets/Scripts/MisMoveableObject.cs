using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MisMoveableObject : MonoBehaviour {

	protected Vector2  _move;
	protected Vector2  _velocity;
	protected Vector2  _deltaPos;
	protected Vector2  _ray;
	// Components
	protected BoxCollider2D _boundingBox;

	// States
	protected bool _isOnGround;

	// Moveable object configurations
	public int   _raysAmount;
	public float _moveSpeed;
	public float _jumpSpeed;
	public float _friction;
	public bool  _applyGravity;
	public bool  _detectHorCollision;
	public bool  _detectVerCollision;

	private Dictionary<Vector2, RaycastHit2D> _collisions;

	// Methods to access properties
	public Vector2 GetDeltaPos() {  return _deltaPos; }

	protected virtual void Start() {

		_boundingBox = GetComponent<BoxCollider2D> ();
		_collisions = new Dictionary<Vector2, RaycastHit2D> ();
	}

	// Update is called once per frame
	protected virtual void FixedUpdate () {

		// Add gravity and friction
		CalculateVelocity ();

		// Detect collisiong and modify velocity vector
		DetectCollision ();

		// Detect collision and integrate velocity to position
		transform.Translate(_deltaPos);
	}
		
	private void CalculateVelocity() {

		_velocity.x = Mathf.Lerp(_velocity.x, _move.x * _moveSpeed * Time.deltaTime, _friction);
		_velocity.x = Mathf.Clamp (_velocity.x, -MisConstants.MAX_SPEED, MisConstants.MAX_SPEED);

		if (_applyGravity)
			_velocity.y += MisConstants.GRAVITY * Time.deltaTime;
		
		_velocity.y += _move.y * _jumpSpeed * Time.deltaTime;
		_velocity.y = Mathf.Clamp (_velocity.y, -MisConstants.MAX_SPEED, MisConstants.MAX_SPEED);
	}

	private void DetectCollision() {
		
		Vector2 size = _boundingBox.bounds.size;
		Vector2 center = _boundingBox.offset;
		Vector2 entityPosition = _boundingBox.bounds.center;

		float deltaY = _velocity.y;
		if (_detectVerCollision)
			deltaY = DetectVerticalCollision (entityPosition, center, size, _raysAmount);

		float deltaX = _velocity.x;
		if (_detectHorCollision && _velocity.x != 0f)
			deltaX = DetectHorizontalCollision (entityPosition, center, size, _raysAmount);

		_deltaPos.x = deltaX;
		_deltaPos.y = deltaY;
	}

	private float DetectHorizontalCollision(Vector2 entityPosition, Vector2 center, Vector2 size, int nRays) {

		float deltaX = _velocity.x;

		int rayPos = 0;
		for (; rayPos < nRays; rayPos++)
			if(xAxisRaycasts (entityPosition, center, size, rayPos, ref deltaX))
				break;

		return deltaX;
	}

	private bool xAxisRaycasts(Vector2 entityPosition, Vector2 center, Vector2 size, int i, ref float deltaX) {

		float dirX = transform.localScale.x;

		_ray.x = entityPosition.x + (center.x + size.x / 2f) * dirX;
		_ray.y = (entityPosition.y + _velocity.y + center.y - size.y / 2f) + size.y / 2f * i;

		RaycastHit2D hit = Physics2D.Raycast (_ray, Vector2.right * dirX, Mathf.Abs (deltaX));
//		Debug.DrawRay (_ray, new Vector2 (dirX, 0));

		if (hit.collider) {

			if (!hit.collider.isTrigger) {

				_velocity.x = 0f;

				float distance = Mathf.Abs (_ray.x - hit.point.x);

				if (distance >= MisConstants.PLAYER_SKIN)

					deltaX = (distance - MisConstants.PLAYER_SKIN) * dirX;
				else
					deltaX = 0f;

				if (!_collisions.ContainsKey(hit.transform.position)) {

					_collisions [hit.transform.position] = hit;
					DidEnterCollision (hit);
				} 
				else
					DidStayCollision (_collisions [hit.transform.position]);

				return true;
			}

			DidEnterEventCollision (hit);
		}

		foreach (RaycastHit2D col in _collisions.Values)
			DidExitCollision (col);

		_collisions.Clear ();

		return false;
	}

	private float DetectVerticalCollision(Vector2 entityPosition, Vector2 center, Vector2 size, int nRays) {

		_isOnGround = false;

		float deltaY = _velocity.y;

		if (transform.localScale.x == 1f) {
			for (float i = nRays - 1f; i >= 0f; i--)
				if(yAxisRaycasts (entityPosition, center, size, i, ref deltaY))
					break;
		} 
		else {
			for (float j = 0f; j < nRays; j++)
				if(yAxisRaycasts (entityPosition, center, size, j, ref deltaY))
					break;
		}

		return deltaY;
	}

	private bool yAxisRaycasts(Vector2 entityPosition, Vector2 center, Vector2 size, float i, ref float deltaY) {

		float dirX = -transform.localScale.x;
		float dirY =  Mathf.Sign(_velocity.y);

		_ray.x = (entityPosition.x + center.x * -dirX - size.x / 2f) + size.x / 2f * i;
		_ray.y = entityPosition.y + center.y + size.y / 2f * dirY;

		RaycastHit2D hit = Physics2D.Raycast(_ray, Vector2.up * dirY, Mathf.Abs(deltaY));
//		Debug.DrawRay(_ray,  Vector2.up * dirY);

		if (hit.collider) {

			if (!hit.collider.isTrigger) {

				_isOnGround = true;
				_velocity.y = 0f;

				float distance = Mathf.Abs (_ray.y - hit.point.y);

				if (distance >= MisConstants.PLAYER_SKIN)

					deltaY = distance * dirY + MisConstants.PLAYER_SKIN;
				else
					deltaY = 0f;

				if (!_collisions.ContainsKey(hit.transform.position)) {

					_collisions [hit.transform.position] = hit;
					DidEnterCollision (hit);
				} 
				else
					DidStayCollision (_collisions [hit.transform.position]);

				return true;
			}

			DidEnterEventCollision (hit);
		}

		foreach (RaycastHit2D col in _collisions.Values)
			DidExitCollision (col);

		_collisions.Clear ();

		return false;
	}

	protected virtual void DidEnterEventCollision(RaycastHit2D hit) {

	}

	protected virtual void DidEnterCollision(RaycastHit2D hit) {

		Debug.Log ("enter");
	}

	protected virtual void DidStayCollision(RaycastHit2D hit) {


	}

	protected virtual void DidExitCollision(RaycastHit2D hit) {

		Debug.Log ("exit");
	}

	protected void Flip(float dir) {

		if (dir != 0f) {
				
			Vector3 temp = transform.localScale;
			temp.x = Mathf.Sign (dir);
			transform.localScale = temp;
		}
	}

	protected bool IsFlipped() {

		return transform.localScale.x == -1f;
	}

	protected bool IsJumping() {

		return _velocity.y > 0f;
	}

	protected bool IsFalling() {

		return _velocity.y < 0f && !_isOnGround;
	}

	protected bool IsRunning() {

		return _velocity.x != 0f;
	}
}