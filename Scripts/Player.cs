using Godot;

namespace Galygun;

public partial class Player : Godot.CharacterBody3D
{
	private const float AirControlSpeed = 1.0f;
	private const float WalkSpeed = 3.0f;
	private const float RunSpeed = 6.0f;
	private const float CrouchSpeed = 2.0f;
	private const float JumpVelocity = 4.5f;
	private const float CameraSensitivity = 0.5f;

	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private float _linearDamping = ProjectSettings.GetSetting("physics/3d/default_linear_damp").AsSingle();
	private Camera3D _camera;
	private Gun _gun;
	private CollisionShape3D _collisionShape;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_gun = FindChild("Gun") as Gun;
		_collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		var crouching = Input.IsActionPressed("crouch");
		var running = Input.IsActionPressed("run") && !crouching;

		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor() && !crouching)
			velocity.Y = JumpVelocity;

		if (IsOnFloor())
		{
			var inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
			var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				var speed = crouching ? CrouchSpeed : running ? RunSpeed : WalkSpeed;
				velocity.X = direction.X * speed;
				velocity.Z = direction.Z * speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, WalkSpeed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, WalkSpeed);
			}
		}
		else
		{
			var inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
			var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				//TODO: Currently accepts new direction. Should instead move toward the direction to provide a more limited air control.
				var momentum = new Vector3(velocity.X, 0, velocity.Z).Length();
				direction *= Mathf.Max(momentum, AirControlSpeed);
				velocity.X = direction.X;
				velocity.Z = direction.Z;
			}
		}

		var collisionCapsule = _collisionShape.Shape as CapsuleShape3D;
		collisionCapsule!.Height = Input.IsActionPressed("crouch") ? 1.2f : 2.0f;

		var linearDampingTimesDeltaSeconds = _linearDamping * (float)delta;
		var linearVelocityMultiplier = new Vector3(
			Mathf.Max(0.0f, 1.0f - linearDampingTimesDeltaSeconds),
			Mathf.Max(0.0f, 1.0f - linearDampingTimesDeltaSeconds),
			Mathf.Max(0.0f, 1.0f - linearDampingTimesDeltaSeconds));
			
		Velocity = velocity * linearVelocityMultiplier;
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