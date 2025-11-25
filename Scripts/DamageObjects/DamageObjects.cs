using Godot;

public partial class DamageObjects : Node3D
{
    [Export] public int Damage = 20;
    [Export] public float DamageInterval = 0.2f;

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

        // Create a timer
        _timer = new Timer
        {
            WaitTime = DamageInterval,
            OneShot = false,
            Autostart = false
        };
        AddChild(_timer);
        _timer.Timeout += OnTimerTimeout;

        // Connect signals
        area.BodyEntered += OnBodyEntered;
        area.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is Player player)
        {
            _player = player;
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
        _player?.TakeDamage(Damage);
    }
}
