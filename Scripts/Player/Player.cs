using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float StandardDamage = 10.0f;
	[Export]
	public float PowerDamage = 500.0f;
	[Export]
	public float Speed = 15.0f;
	[Export]
	public float SuperSpeed = 30.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public float GravityMultiplier = 3.0f;
	
	private bool _inflictedMeleeDamage = false;
	private AnimationPlayer meleeAnim;
	private Area3D hitbox;

	public override void _Ready()
	{
		meleeAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		hitbox = GetNode<Area3D>("Head/Camera3D/Hitbox");
		//_speed = Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * GravityMultiplier;
		}

		// Handle Jump.
		if (Input.IsActionPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("Player_left", "Player_right", "Player_forward", "Player_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		OnMeleeAttack();
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
				if (body is Enemy enemyStrength && body.IsInGroup("EnemyStrength"))
				{
					enemyStrength.strengthHealth -= StandardDamage;
					enemyStrength.PowerDamage

					GD.Print($"Enemy health: {enemyStrength.EnemyStrengthHealth}");
				}

				if (body is Enemy enemyWallStrength && body.IsInGroup("EnemyWallStrength"))
				{
					enemyWallStrength.WallHealth -= StandardDamage;

					GD.Print($"Enemy Wall health: {enemyWallStrength.WallHealth}");
				}

				if (body is Enemy enemySpeed && body.IsInGroup("EnemySpeed"))
				{
					enemySpeed.EnemySpeedHealth -= StandardDamage;
					
					GD.Print($"Super speed health: {enemySpeed.EnemySpeedHealth}");
				}

				/*if (body.IsInGroup("EnemyStrength"))
				{
					if (body is Enemy enemy)
					{
						enemy.EnemyStrengthHealth -= MeleeDamage;

						GD.Print($"Enemy health: {enemy.EnemyStrengthHealth}");
					}

					//_speed = Speed;
					//_strength = PowerLevel;
				}

				else if (body.IsInGroup("EnemyWall"))
				{
					if (body is Enemy enemy)
					{
						enemy.WallHealth -= MeleeDamage;

						GD.Print($"Enemy Wall health: {enemy.WallHealth}");
					}
				}

				else if (body.IsInGroup("EnemySpeed"))
				{
					if (body is Enemy enemy)
					{
						enemy.EnemySpeedHealth -= _strength;
						
						GD.Print($"Super speed health: {enemy.EnemySpeedHealth}");
					}

					_speed = SuperSpeed;
					_strength = MeleeDamage;
				}*/
			}

			_inflictedMeleeDamage = true;
		}
	}
}
