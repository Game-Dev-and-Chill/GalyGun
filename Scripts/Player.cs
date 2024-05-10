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
	private Node3D _gunHolder;
	private Gun _gun;
	private ShapeCast3D _uncrouchCheck;
	private AnimationPlayer _animationPlayer;

	private bool _crouching;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_gunHolder = FindChild("GunHolder") as Node3D;
		_gun = _gunHolder!.GetNode<Gun>("Gun");
		_uncrouchCheck = GetNode<ShapeCast3D>("UncrouchCheck");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		var crouch = Input.IsActionPressed("crouch");
		var running = Input.IsActionPressed("run") && !_crouching;

		if (crouch != _crouching)
		{
			if (crouch)
			{
				_animationPlayer.Play("crouch", -1, 5f, false);
				_crouching = true;
			}
			else
			{
				if (!_uncrouchCheck.IsColliding())
				{
					_animationPlayer.Play("crouch", -1, -5f, true);
					_crouching = false;
				}
			}
		}

		
		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor() && !_crouching)
			velocity.Y = JumpVelocity;

		if (IsOnFloor())
		{
			var inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
			var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				var speed = _crouching ? CrouchSpeed : running ? RunSpeed : WalkSpeed;
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

		var linearDampingTimesDeltaSeconds = _linearDamping * (float)delta;
		var linearVelocityMultiplier = new Vector3(
			Mathf.Max(0.0f, 1.0f - linearDampingTimesDeltaSeconds),
			Mathf.Max(0.0f, 1.0f - linearDampingTimesDeltaSeconds),
			Mathf.Max(0.0f, 1.0f - linearDampingTimesDeltaSeconds));
			
		Velocity = velocity * linearVelocityMultiplier;

		var centerViewport = GetViewport().GetWindow().Size / 2;
		var spaceState = GetWorld3D().DirectSpaceState;
		var rayHit = spaceState.IntersectRay(new PhysicsRayQueryParameters3D
		{
			From = _camera.ProjectRayOrigin(centerViewport),
			To = _camera.ProjectRayNormal(centerViewport) * 1000,
			CollisionMask = 2,
			CollideWithBodies = true
		});
		if (rayHit.Count > 0)
		{
			_gunHolder.LookAt(rayHit["position"].AsVector3(), Vector3.Up);
		}
		
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