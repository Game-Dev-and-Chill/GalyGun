using Godot;
using System;

public partial class CharacterBody3D : Godot.CharacterBody3D
{
	public Camera3D Camera	{ get; set; }
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	public const float camera_sensitivity = 0.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		Camera = GetNode<Camera3D>("Camera3D");
		// Lock the mouse cursor to the window.
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		CameraToMouse(@event);
	}

	private void CameraToMouse(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseEvent)
		{
			RotateY(-Mathf.DegToRad(mouseEvent.Relative.X * camera_sensitivity));
			Camera.RotateX(-Mathf.DegToRad(mouseEvent.Relative.Y * camera_sensitivity));
			Camera.Rotation = new Vector3(Mathf.Clamp(Camera.Rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2), Camera.Rotation.Y, Camera.Rotation.Z);
		}
	}
}
