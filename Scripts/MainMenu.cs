using Godot;
using System;

public partial class MainMenu : Control
{
	private AudioController _audio;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_audio = GetNode<AudioController>("/root/AudioController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/Level.tscn");
		_audio.StartGame();
	}

	public void OnOptionsPressed()
	{
		GD.Print($"Options pressed");
	}

	public void OnExitPressed()
	{
		GetTree().Quit();
	}
}
