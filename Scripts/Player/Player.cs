using Godot;

public partial class Player : CharacterBody3D
{
	[Export] public float StandardDamage = 10.0f;
	[Export] public float PowerDamage = 500.0f;
	[Export] public float StandardSpeed = 15.0f;
	[Export] public float SuperSpeed = 30.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float GravityMultiplier = 3.0f;
	[Export] public float PushForce = 0.2f;
	[Export] public string PushGroupName = "EnemyBlock";

	public float currentDamage = 10.0f;
	public float currentSpeed = 15.0f;
	public bool CanPushEnemyBlocks { get; private set; } = false;
	
	private bool _inflictedMeleeDamage = false;
	private AnimationPlayer meleeAnim;
	private Area3D hitbox;
	private Vector3 _lastMoveDirection = Vector3.Zero;

	public override void _Ready()
	{
		meleeAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		hitbox = GetNode<Area3D>("Head/Camera3D/Hitbox");
		currentDamage = StandardDamage;

		UpDirection = Vector3.Up;
		// IMPORTANT FIXES
		FloorMaxAngle = Mathf.DegToRad(25);  // Prevent walls from counting as floor
	}

	public void UnlockPushAbility()
	{
		if (!CanPushEnemyBlocks)
			CanPushEnemyBlocks = true;
	}

	public void LockPushAbility()
	{
		if (CanPushEnemyBlocks)
		{
			CanPushEnemyBlocks = false;
			_lastMoveDirection = Vector3.Zero;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * GravityMultiplier;
		}

		// Jump
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Input
		Vector2 inputDir = Input.GetVector("Player_left", "Player_right", "Player_forward", "Player_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * currentSpeed;
			velocity.Z = direction.Z * currentSpeed;
			_lastMoveDirection = direction;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currentSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, currentSpeed);
			_lastMoveDirection = Vector3.Zero;
		}

		Velocity = velocity;

		// CRITICAL FIX: Tell Godot what "up" is
		MoveAndSlide();

		OnMeleeAttack();
		HandlePushCollisions();
	}

	private void OnMeleeAttack()
	{
		if (Input.IsActionJustPressed("Player_attack"))
		{
			meleeAnim.Play("Attack");
			_inflictedMeleeDamage = false;
		}

		if (meleeAnim.IsPlaying() && meleeAnim.CurrentAnimation == "Attack" && !_inflictedMeleeDamage)
		{
			var bodies = hitbox.GetOverlappingBodies();
			foreach (var body in bodies)
			{
				if (body.IsInGroup("EnemyStrength"))
				{
					if (body is Enemy enemy)
					{
						enemy.strengthHealth -= currentDamage;
						GD.Print($"Enemy health: {enemy.strengthHealth}");
					}
				}

				if (body.IsInGroup("EnemySpeed"))
				{
					if (body is Enemy enemy)
					{
						enemy.speedHealth -= currentDamage;
						GD.Print($"Super speed health: {enemy.speedHealth}");
					}
				}

				if (body.IsInGroup("EnemyWall"))
				{
					if (body is Enemy enemy)
					{
						enemy.wallHealth -= currentDamage;
						GD.Print($"Enemy Wall health: {enemy.wallHealth}");
					}
				}

				if (body.IsInGroup("EnemyInteract"))
				{
					if (body is Enemy enemy)
					{
						enemy.interactHealth -= currentDamage;
						GD.Print($"Enemy Interact health: {enemy.interactHealth}");
					}
				}
			}

			_inflictedMeleeDamage = true;
		}
	}

	private void HandlePushCollisions()
	{
		if (!CanPushEnemyBlocks || string.IsNullOrEmpty(PushGroupName) || _lastMoveDirection == Vector3.Zero)
			return;

		int collisionCount = GetSlideCollisionCount();
		if (collisionCount == 0)
			return;

		for (int i = 0; i < collisionCount; i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision?.GetCollider();
			if (collider is RigidBody3D rigidBody && rigidBody.IsInGroup(PushGroupName))
			{
				rigidBody.ApplyImpulse(_lastMoveDirection.Normalized() * PushForce);
			}
		}
	}

}
