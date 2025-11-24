using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float StandardDamage = 10.0f;
	[Export]
	public float PowerDamage = 500.0f;
	[Export]
	public float StandardSpeed = 15.0f;
	[Export]
	public float SuperSpeed = 30.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public float GravityMultiplier = 3.0f;

	public float currentDamage = 10.0f;
	public float currentSpeed = 15.0f;
	
	private bool _inflictedMeleeDamage = false;
	private AnimationPlayer meleeAnim;
	private Area3D hitbox;

	public override void _Ready()
	{
		meleeAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		hitbox = GetNode<Area3D>("Head/Camera3D/Hitbox");
		currentDamage = StandardDamage;
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
			velocity.X = direction.X * currentSpeed;
			velocity.Z = direction.Z * currentSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currentSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, currentSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();

		OnMeleeAttack();
	}

	// Apply damage to enemies
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
				/*if (body is Enemy enemyStrength && body.IsInGroup("EnemyStrength"))
				{
					enemyStrength.strengthHealth -= currentDamage;

					GD.Print($"Enemy health: {enemyStrength.strengthHealth}");
				}

				if (body is Enemy enemySpeed && body.IsInGroup("EnemySpeed"))
				{
					enemySpeed.speedHealth -= currentDamage;
					
					GD.Print($"Super speed health: {enemySpeed.speedHealth}");
				}

				if (body is Enemy enemyWallStrength && body.IsInGroup("EnemyWall"))
				{
					enemyWallStrength.wallHealth -= currentDamage;

					GD.Print($"Enemy Wall health: {enemyWallStrength.wallHealth}");
				}*/

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
			}

			_inflictedMeleeDamage = true;
		}
	}
}
