using Godot;

namespace Galygun.Scripts;

public partial class Reticle : CenterContainer
{
	[Export] public Player Player;
	[Export] public float ReticleSpeed = 0.25f;
	[Export] public float ReticleDistance = 2.0f;
	[Export] public float DotRadius = 1.0f;
	[Export] public Color ReticleColor = Colors.White;
	[Export] public float ReticleLineLength = 8.0f;

	private float _currentReticleDistance;

	public override void _Ready()
	{
		_currentReticleDistance = ReticleDistance;
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		var speed = Player.GetRealVelocity().Length();
		_currentReticleDistance = ReticleDistance + Mathf.Lerp(_currentReticleDistance, ReticleDistance * speed, ReticleSpeed);

		DrawCircle(Vector2.Zero, 1, Colors.White);
		DrawLine(new Vector2(_currentReticleDistance, 0), new Vector2(_currentReticleDistance + ReticleLineLength, 0), Colors.White, 2);
		DrawLine(new Vector2(-_currentReticleDistance, 0), new Vector2(-(_currentReticleDistance + ReticleLineLength), 0), Colors.White, 2);
		DrawLine(new Vector2(0, _currentReticleDistance), new Vector2(0, _currentReticleDistance + ReticleLineLength), Colors.White, 2);
		DrawLine(new Vector2(0, -_currentReticleDistance), new Vector2(0, -(_currentReticleDistance + ReticleLineLength)), Colors.White, 2);
	}
}