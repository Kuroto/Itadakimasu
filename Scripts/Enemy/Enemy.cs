using Godot;
using System;

public partial class Enemy : RigidBody3D
{
	public float strengthHealth = 100.0f; // Health of strength enemies.
	public float speedHealth = 100.0f; // Health of speed enemies.
	public float wallHealth = 5000.0f; // Health of walls/boulders.

	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var playerNode = GetTree().GetFirstNodeInGroup("Player") as Player;
  		
		if (playerNode != null)
	  	player = playerNode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player == null)
		{
			return;
		}

		if (strengthHealth <= 0)
		{
			player.currentDamage = player.PowerDamage;
			player.currentSpeed = player.StandardSpeed;
			QueueFree();
		}

		if (speedHealth <= 0)
		{
			player.currentDamage = player.StandardDamage;
			player.currentSpeed = player.SuperSpeed;
			QueueFree();
		}

		if (wallHealth <= 0)
		{
			QueueFree();
		}
	}
}
