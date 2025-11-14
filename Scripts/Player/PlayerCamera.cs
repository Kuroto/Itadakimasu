using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	[Export] public float MouseSensitivityX = 0.1f;

	public override void _Ready()
	{
		// Captures mouse into the game view. Necessary otherwise the mouse will be foating.
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			GetParent<Node3D>()?.RotateY(Mathf.DegToRad(-motion.Relative.X * MouseSensitivityX));
			RotateX(Mathf.DegToRad(-motion.Relative.Y * MouseSensitivityX));

			var rot = Rotation; // radians
			rot.X = Mathf.Clamp(rot.X, Mathf.DegToRad(-90f), Mathf.DegToRad(90f));
			Rotation = rot;
		}
	}
}
