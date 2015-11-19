using UnityEngine;
using System.Collections;

public class MisCharacter : MonoBehaviour {

	protected Animator _animator;
	protected Vector2  _velocity;

	protected Vector2 _deltaPos;
	public Vector2 GetDeltaPos() {
		return _deltaPos;
	}

	protected bool  _isAttacking;
	protected bool  _isOnGround;
	protected bool  _isDead;
	protected float _moveX;
	
	public  float _moveSpeed;
	public  float _jumpSpeed;
	
	// Use this for initialization
	public virtual void Start () {
		
		_animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
		if (!_isDead) {

			// Add gravity and friction
			CalculateVelocity ();
		
			// Detect collisiong and modify velocity vector
			_deltaPos = DetectCollision ();

			// Detect collision and integrate velocity to position
			transform.position += (Vector3)_deltaPos;
		
			_animator.SetBool ("isRunning", IsRunning () && !_isAttacking);
			_animator.SetBool ("isJumping", IsJumping () && !_isAttacking);

			_animator.SetBool ("isShooting", _isAttacking);
		}
	}
	
	private void CalculateVelocity() {
		
		_velocity.x += _moveSpeed * _moveX;
		_velocity.y += MisConstants.GRAVITY;
		
		if (_isOnGround)
			
			ApplyFriction (Vector2.up, MisConstants.FRICTION_COEF_GROUND);
		else
			ApplyFriction (Vector2.up, MisConstants.FRICTION_COEF_AIR);

		_velocity.x = Mathf.Clamp (_velocity.x, -MisConstants.MAX_SPEED, MisConstants.MAX_SPEED);
		_velocity.y = Mathf.Clamp (_velocity.y, -MisConstants.MAX_SPEED, MisConstants.MAX_SPEED);
	}
	
	private Vector2 DetectCollision() {
		
		BoxCollider2D col = GetComponent<BoxCollider2D> ();
		
		Vector2 size = col.bounds.size;
		Vector2 center = col.offset;
		Vector2 entityPosition = transform.position;
		
		float correY = DetectVerticalCollition (entityPosition, center, size, MisConstants.COLLISION_RAYS);
		
		float correX = 0f;
		if(_velocity.x != 0f)
			correX = DetectHorizontalCollition (entityPosition, center, size, MisConstants.COLLISION_RAYS);

		return new Vector2(correX, correY);
	}
	
	private float DetectHorizontalCollition(Vector2 entityPosition, Vector2 center, Vector2 size, int nRays) {
		
		float deltaX = _velocity.x * Time.deltaTime;
		
		float startPos = (!_isOnGround ? -0.5f : 0f);
		for (float i = startPos; i < (float)nRays + startPos; i++) {
			
			float dirX = transform.localScale.x;
			
			float x = entityPosition.x + (center.x + size.x / 2f) * dirX;
			float y = (entityPosition.y + center.y - size.y / 2f) + size.y / 2 * i;
			
			Vector2 rayX = new Vector2 (x, y);
			
			RaycastHit2D hit = Physics2D.Raycast (rayX, new Vector2 (dirX, 0), Mathf.Abs (deltaX));

			if (hit.collider) {

				Debug.DrawRay (rayX, new Vector2 (dirX, 0));

				if (!hit.collider.isTrigger) {
				
					float distance = Mathf.Abs (x - hit.point.x);
				
					if (distance >= MisConstants.PLAYER_SKIN)

						deltaX = (distance - MisConstants.PLAYER_SKIN) * dirX;
					else
						deltaX = 0f;

					return deltaX;
				}

				TriggerEvent (hit.collider);
			}
		}

		return deltaX;
	}
	
	private float DetectVerticalCollition(Vector2 entityPosition, Vector2 center, Vector2 size, int nRays) {
		
		_isOnGround = false;
		
		float deltaY = _velocity.y * Time.deltaTime;

		float dirX = -transform.localScale.x;
		float dirY = Mathf.Sign(deltaY);

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

		if (hit.collider) {

			Debug.DrawRay(new Vector2(x, y),  new Vector2(0, dirY));
		
			if (!hit.collider.isTrigger) {
			
				_isOnGround = true;
				_velocity.y = 0f;
			
				float distance = Mathf.Abs (y - hit.point.y);
			
				if (distance >= MisConstants.PLAYER_SKIN)

					deltaY = distance * dirY + MisConstants.PLAYER_SKIN;
				else
					deltaY = 0f;

				return true;
			}
		
			TriggerEvent (hit.collider);
		}

		return false;
	}
	
	private void ApplyFriction(Vector2 normal, float frictionCoef) {
		
		Vector2 tangent = new Vector2(normal.y, -normal.x); 
		_velocity -= tangent * Vector2.Dot(_velocity, tangent) * frictionCoef;

		// Avoid really small floating points
		if (Mathf.Abs(_velocity.x) < 0.05f)
			_velocity.x = 0f;
	}

	protected virtual void TriggerEvent(Collider2D collider) {
	
	}

	protected bool IsFlipped() {
		
		return (transform.localScale.x == -1f);
	}
	
	protected bool IsJumping() {
		
		return (_velocity.y > 0f);
	}
	
	protected bool IsRunning() {
		
		return (_moveX != 0f);
	}
}
