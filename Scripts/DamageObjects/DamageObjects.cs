using Godot;

public partial class DamageObjects : Node3D
{
	[Export] public int Damage = 10;
	[Export] public float DamageInterval = 0.2f; // damage every 0.2 seconds

	private Timer _timer;
	private Player _player;

	public override void _Ready()
	{
		var area = GetNode<Area3D>("Area3D");
		if (area == null)
		{
			GD.PrintErr("DamageObject requires an Area3D child!");
			return;
		}

		// Create timer for repeated damage
		_timer = new Timer
		{
			WaitTime = DamageInterval,
			Autostart = false,
			OneShot = false
		};

		AddChild(_timer);
		_timer.Timeout += OnTimerTimeout;

		// Connect area signals
		area.BodyEntered += OnBodyEntered;
		area.BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Player player)
		{
			_player = player;

			// Deal immediate damage safely
			_player.CallDeferred(nameof(Player.TakeDamage), Damage);

			// Start repeated damage
			if (IsInsideTree())
				_timer.Start();
		}
	}

	private void OnBodyExited(Node3D body)
	{
		if (body == _player)
		{
			_timer.Stop();
			_player = null;
		}
	}

	private void OnTimerTimeout()
	{
		if (_player != null && _player.IsInsideTree())
		{
			_player.TakeDamage(Damage);
		}
	}
}
