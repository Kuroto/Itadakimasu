using Godot;

public partial class HUD : CanvasLayer
{
    private ProgressBar _healthBar;

    public override void _Ready()
    {
        _healthBar = GetNode<ProgressBar>("HealthBar");
    }

    public void SetHealth(int value)
    {
        _healthBar.Value = value;
    }
}