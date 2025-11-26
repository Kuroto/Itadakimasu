using Godot;
using System;

public partial class Enemy : RigidBody3D
{
	public float strengthHealth = 100.0f; // Health of strength enemies.
	public float speedHealth = 100.0f; // Health of speed enemies.
	public float wallHealth = 5000.0f; // Health of walls/boulders.
	public float interactHealth = 100.0f; // Health of the interactability enemies.

	Player player;

	private AudioController _audio;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var playerNode = GetTree().GetFirstNodeInGroup("Player") as Player;
		_audio = GetNode<AudioController>("/root/AudioController");
  		
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
			if (player.CanPushEnemyBlocks)
			{
				player.LockPushAbility();
			}

			player.currentDamage = player.PowerDamage;
			player.currentSpeed = player.StandardSpeed;
			_audio.PlayDeath();
			QueueFree();
		}

		if (speedHealth <= 0)
		{
			if (player.CanPushEnemyBlocks)
			{
				player.LockPushAbility();
			}

			player.currentDamage = player.StandardDamage;
			player.currentSpeed = player.SuperSpeed;
			_audio.PlayDeath();
			QueueFree();
		}

		if (wallHealth <= 0)
		{
			if (player.CanPushEnemyBlocks)
			{
				player.LockPushAbility();
			}

			QueueFree();
		}

		if (interactHealth <= 0)
		{
			if (!player.CanPushEnemyBlocks)
			{
				player.UnlockPushAbility();
			}

			player.currentDamage = player.StandardDamage;
			player.currentSpeed = player.StandardSpeed;
			_audio.PlayDeath();
			QueueFree();
		}
	}
}
