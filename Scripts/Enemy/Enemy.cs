using Godot;
using System;

public partial class Enemy : RigidBody3D
{
	public float strengthHealth = 100.0f; // Health of strength enemies.
	public float speedHealth = 100.0f; // Health of speed enemies.
	public float wallHealth = 5000.0f; // Health of walls/boulders.

	private float _standardDamage = 10.0f;
	private float _powerDamage = 500.0f;
	private float _superSpeed = 30.0f;

	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (EnemyStrengthHealth <= 0)
		{
			player.MeleeDamageBase = _powerDamage;
			player.Speed = 
            QueueFree();
        }

		if (WallHealth <= 0)
		{
            QueueFree();
        }

		if (EnemySpeedHealth <= 0)
		{
			player.Speed = _superSpeed;
			player.MeleeDamage = _standardDamage;
		}
    }
}
