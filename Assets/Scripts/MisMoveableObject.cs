using UnityEngine;
using System.Collections;

public class MisMoveableObject : MonoBehaviour {

	protected Vector2  _move;
	protected Vector2  _velocity;

	protected BoxCollider2D _boundingBox;

	protected bool _isOnGround;

	protected Vector2  _deltaPos;
	public    Vector2  GetDeltaPos() {  return _deltaPos; }

	// Moveable object configurations
	public int   _raysAmount;
	public float _moveSpeed;
	public float _jumpSpeed;
	public bool  _applyGravity;
	public bool  _detectHorCollision;
	public bool  _detectVerCollision;

	protected virtual void Start() {

		_boundingBox = GetComponent<BoxCollider2D> ();
	}

	// Update is called once per frame
	protected virtual void FixedUpdate () {

		// Add gravity and friction
		CalculateVelocity ();

		// Detect collisiong and modify velocity vector
		_deltaPos = DetectCollision ();

		// Detect collision and integrate velocity to position
		transform.Translate(_deltaPos);
	}
		
	private void CalculateVelocity() {

		_velocity.x = _move.x * _moveSpeed * Time.deltaTime;
		_velocity.x = Mathf.Clamp (_velocity.x, -MisConstants.MAX_SPEED, _raysAmount);

		if (_applyGravity)
			_velocity.y += MisConstants.GRAVITY * Time.deltaTime;
		
		_velocity.y += _move.y * _jumpSpeed * Time.deltaTime;
		_velocity.y = Mathf.Clamp (_velocity.y, -MisConstants.MAX_SPEED, _raysAmount);
	}

	private Vector2 DetectCollision() {

		Vector2 size = _boundingBox.bounds.size;
		Vector2 center = _boundingBox.offset;
		Vector2 entityPosition = transform.position;

		float correY = _velocity.y;
		if (_detectVerCollision)
			correY = DetectVerticalCollition   (entityPosition, center, size, _raysAmount);

		float correX = _velocity.x;
		if (_detectHorCollision && _move.x != 0f)
			correX = DetectHorizontalCollition (entityPosition, center, size, _raysAmount);
		
		return new Vector2(correX, correY);
	}

	private float DetectHorizontalCollition(Vector2 entityPosition, Vector2 center, Vector2 size, int nRays) {

		float deltaX = _velocity.x;

		float dirX = transform.localScale.x;

		for (float i = 0f; i < nRays; i++)
			if(xAxisRaycasts (entityPosition, center, size, i, ref deltaX, dirX))
				break;

		return deltaX;
	}

	private bool xAxisRaycasts(Vector2 entityPosition, Vector2 center, Vector2 size, float i, ref float deltaX, float dirX) {

		float x = entityPosition.x + (center.x + size.x / 2f) * dirX;
		float y = (entityPosition.y + center.y - size.y / 2f) + size.y / 2 * i;

		Vector2 rayX = new Vector2 (x, y);

		RaycastHit2D hit = Physics2D.Raycast (rayX, new Vector2 (dirX, 0), Mathf.Abs (deltaX));
		Debug.DrawRay (rayX, new Vector2 (dirX, 0));

		if (hit.collider) {

			if (!hit.collider.isTrigger) {

				_velocity.x = 0f;

				float distance = Mathf.Abs (x - hit.point.x);

				if (distance >= MisConstants.PLAYER_SKIN)

					deltaX = (distance - MisConstants.PLAYER_SKIN) * dirX;
				else
					deltaX = 0f;

				DidEnterCollision (hit);

				return true;
			}

			DidEnterEventCollision (hit);
		}

		return false;
	}

	private float DetectVerticalCollition(Vector2 entityPosition, Vector2 center, Vector2 size, int nRays) {

		_isOnGround = false;

		float deltaY = _velocity.y;

		float dirX = -transform.localScale.x;
		float dirY =  Mathf.Sign(_velocity.y);

		if (dirX == 1f) {
			for (float i = nRays - 1f; i >= 0f; i--)
				if(yAxisRaycasts (entityPosition, center, size, i, ref deltaY, dirX, dirY))
					break;
		} 
		else {
			for (float j = 0f; j < nRays; j++)
				if(yAxisRaycasts (entityPosition, center, size, j, ref deltaY, dirX, dirY))
					break;
		}

		return deltaY;
	}

	private bool yAxisRaycasts(Vector2 entityPosition, Vector2 center, Vector2 size, float i, ref float deltaY, float dirX, float dirY) {

		float x = (entityPosition.x + center.x * -dirX - size.x / 2f) + size.x / 2f * i;
		float y = entityPosition.y + center.y + size.y / 2f * dirY;

		Vector2 rayY = new Vector2(x, y);

		RaycastHit2D hit = Physics2D.Raycast(rayY, new Vector2(0, dirY), Mathf.Abs(deltaY));
		Debug.DrawRay(new Vector2(x, y),  new Vector2(0, dirY));

		if (hit.collider) {

			if (!hit.collider.isTrigger) {

				_isOnGround = true;
				_velocity.y = 0f;

				float distance = Mathf.Abs (y - hit.point.y);

				if (distance >= MisConstants.PLAYER_SKIN)

					deltaY = distance * dirY + MisConstants.PLAYER_SKIN;
				else
					deltaY = 0f;

				DidEnterCollision (hit);

				return true;
			}

			DidEnterEventCollision (hit);
		}

		return false;
	}

	protected virtual void DidEnterEventCollision(RaycastHit2D hit) {

	}

	protected virtual void DidEnterCollision(RaycastHit2D hit) {

	}

	protected void Flip(float dir) {

		float nextDir = Mathf.Sign (dir);
		transform.localScale = new Vector3 (nextDir, 1f, 1f);
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

		return (_move.x != 0f);
	}

}