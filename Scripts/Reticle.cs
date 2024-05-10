using Godot;
using System;

public partial class Reticle : CenterContainer
{
	public override void _Ready()
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, 1, Colors.White);
		// DrawLine(new Vector2(4, 0), new Vector2(8, 0), Colors.White, 2);
		// DrawLine(new Vector2(-4, 0), new Vector2(-8, 0), Colors.White, 2);
		// DrawLine(new Vector2(0, 4), new Vector2(0, 8), Colors.White, 2);
		// DrawLine(new Vector2(0, -4), new Vector2(0, -8), Colors.White, 2);
	}
}
