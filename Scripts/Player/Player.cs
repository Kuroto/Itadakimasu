using Godot;
using System.Collections.Generic;

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

	public int Health { get; private set; } = 100;
	public float currentDamage = 10.0f;
	public float currentSpeed = 15.0f;
	public bool CanPushEnemyBlocks { get; private set; } = false;

	private bool _inflictedMeleeDamage = false;
	private AnimationPlayer meleeAnim;
	private Area3D hitbox;
	private Vector3 _lastMoveDirection = Vector3.Zero;
	private AudioController _audio;

	// PackedScene for blood particles
	private PackedScene _bloodSplatterScene = ResourceLoader.Load<PackedScene>("res://Scenes/Effects/BloodSplatter.tscn");

	// Active blood particle instances
	private List<GpuParticles3D> _activeBlood = new List<GpuParticles3D>();

	public override void _Ready()
	{
		meleeAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		hitbox = GetNode<Area3D>("Head/Camera3D/Hitbox");
		currentDamage = StandardDamage;
		_audio = GetNode<AudioController>("/root/AudioController");

		// Wall sticking fix
		UpDirection = Vector3.Up;
		FloorMaxAngle = Mathf.DegToRad(45); // Prevent walls from counting as floor
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

	// ---------------- Physics Process ----------------
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * GravityMultiplier;
		}

		// Jump
		if (Input.IsActionPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Movement input
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
		MoveAndSlide();

		OnMeleeAttack();
		HandlePushCollisions();
	}

	// ---------------- Player Attack Logic ----------------
	private void OnMeleeAttack()
	{
		if (Input.IsActionJustPressed("Player_attack"))
		{
			meleeAnim.Play("Attack");
			_inflictedMeleeDamage = false;

			// Start blood emission for all overlapping enemies
			foreach (var body in hitbox.GetOverlappingBodies())
			{
				if (body is Enemy enemy)
				{
					// Skip blood for any node in the "NoBlood" group
					if (enemy.IsInGroup("NoBlood"))
						continue;

					StartBlood(enemy);
				}
			}
		}

		if (meleeAnim.IsPlaying() && meleeAnim.CurrentAnimation == "Attack" && !_inflictedMeleeDamage)
		{
			var bodies = hitbox.GetOverlappingBodies();
			foreach (var body in bodies)
			{
				if (body is Enemy enemy)
				{
					if (body.IsInGroup("EnemyStrength"))
					{
						enemy.strengthHealth -= currentDamage;
						_audio.PlayEat();
						GD.Print($"Enemy health: {enemy.strengthHealth}");
					}

					if (body.IsInGroup("EnemySpeed"))
					{
						enemy.speedHealth -= currentDamage;
						_audio.PlayEat();
						GD.Print($"Super speed health: {enemy.speedHealth}");
					}

					if (body.IsInGroup("EnemyWall"))
					{
						enemy.wallHealth -= currentDamage;
						GD.Print($"Enemy Wall health: {enemy.wallHealth}");
					}

					if (body.IsInGroup("EnemyInteract"))
					{
						enemy.interactHealth -= currentDamage;
						_audio.PlayEat();
						GD.Print($"Enemy Interact health: {enemy.interactHealth}");
					}
				}
			}

			_inflictedMeleeDamage = true;
		}
	}

	// ---------------- Blood Particle Logic ----------------
	private void StartBlood(Node3D target)
	{
		if (_bloodSplatterScene == null || target == null)
			return;

		var b = (GpuParticles3D)_bloodSplatterScene.Instantiate();
		// Attach to scene root (or optionally to enemy for moving blood)
		GetTree().Root.AddChild(b);

		// Set initial position and rotation
		b.GlobalTransform = new Transform3D(target.GlobalTransform.Basis, target.GlobalPosition);

		// Start emitting
		b.Emitting = true;

		_activeBlood.Add(b);

		AutoFreeBlood(b); // <<< Let the blood animation finish naturally
	}

	private void StopBlood()
	{
		foreach (var b in _activeBlood)
		{
			if (b != null && b.IsInsideTree())
			{
				b.Emitting = false;
				b.QueueFree(); // Remove after particles finish
			}
		}

		_activeBlood.Clear();
	}

	private async void AutoFreeBlood(GpuParticles3D blood)
{
	if (blood == null) return;

	float lifetime = (float)blood.Lifetime;

	// Wait for the particles to finish fully
	await ToSignal(GetTree().CreateTimer(lifetime + 0.2f), "timeout");

	if (blood != null && blood.IsInsideTree())
		blood.QueueFree();
}

	// ---------------- Push Logic ----------------
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

	// ---------------- Player Health Damage Logic ----------------
	public void TakeDamage(int amount)
	{
		Health -= amount;
		_audio.PlayPlayerHurt();
		if (Health < 0)
		{
			Health = 0;
		}

		GD.Print($"Player took {amount} damage! Health: {Health}");

		if (Health == 0)
		{
			_audio.PlayPlayerDeath();
			DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
			GetTree().ChangeSceneToFile("res://Scenes/UI/MainMenu.tscn");
			GD.Print("Player is dead!");
		}
	}
}
