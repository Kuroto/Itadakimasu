using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	[Export] public float Sensitivity = 0.2f;

	public override void _Ready()
	{
		// Captures mouse into the game view. Necessary otherwise the mouse will be foating.
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			GetParent<Node3D>()?.RotateY(Mathf.DegToRad(-motion.Relative.X * Sensitivity));
			RotateX(Mathf.DegToRad(-motion.Relative.Y * Sensitivity));

			var rot = Rotation; // radians
			rot.X = Mathf.Clamp(rot.X, Mathf.DegToRad(-90f), Mathf.DegToRad(90f));
			Rotation = rot;
		}
	}
}
