using Godot;
using System;

public partial class AudioController : Node3D
{
	[Export] public bool Mute = false;

	private AudioStreamPlayer _music;
	private AudioStreamPlayer _startGame;
	private AudioStreamPlayer _eat;
	private AudioStreamPlayer _death;
	private AudioStreamPlayer _playerDeath;
	private AudioStreamPlayer _playerHurt;

	public override void _Ready()
	{
		_music = GetNode<AudioStreamPlayer>("Music");
		_startGame = GetNode<AudioStreamPlayer>("StartGame");
		_eat = GetNode<AudioStreamPlayer>("Eat");
		_death = GetNode<AudioStreamPlayer>("Death");
		_playerDeath = GetNode<AudioStreamPlayer>("PlayerDeath");
		_playerHurt = GetNode<AudioStreamPlayer>("PlayerHurt");

		if (!Mute)
			PlayMusic();
	}

	public void PlayMusic()
	{
		if (!Mute && _music != null)
			_music.Play();
	}

	public void StartGame()
	{
		if (!Mute && _startGame != null)
		{
			_startGame.Play();
		}
	}

	public void PlayEat()
	{
		if (!Mute && _eat != null)
			_eat.Play();
	}

	public void PlayDeath()
	{
		if (!Mute && _death != null)
		{
			_death.Play();
		}
	}

	public void PlayPlayerHurt()
	{
		if (!Mute && _playerHurt != null)
		{
			_playerHurt.Play();
		}
	}

	public void PlayPlayerDeath()
	{
		if (!Mute && _playerDeath != null)
		{
			_playerDeath.Play();
		}
	}
}
