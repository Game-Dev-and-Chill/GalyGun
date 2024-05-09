using Godot;

namespace Galygun;

public partial class Player : Godot.CharacterBody3D
{
	private const float Speed = 5.0f;
	private const float JumpVelocity = 4.5f;
	private const float CameraSensitivity = 0.5f;

	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Camera3D _camera;
	private Gun _gun;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_gun = FindChild("Gun") as Gun;

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		var inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Back");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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
		if (@event.IsActionPressed("shoot"))
		{
			_gun.Shoot();
			return;
		}

		switch (@event)
		{
			case InputEventMouseMotion mouseEvent:
				RotateY(-Mathf.DegToRad(mouseEvent.Relative.X * CameraSensitivity));

				var cameraRotation = _camera.Rotation;
				cameraRotation.X = Mathf.Clamp(cameraRotation.X -Mathf.DegToRad(mouseEvent.Relative.Y * CameraSensitivity), -Mathf.Pi / 2, Mathf.Pi / 2);
				_camera.Rotation = cameraRotation;
				break;
		}
	}
}