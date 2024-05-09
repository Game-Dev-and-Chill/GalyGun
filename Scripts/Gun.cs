using Godot;

namespace Galygun;

public partial class Gun : MeshInstance3D
{
	private PackedScene _bulletScene = GD.Load<PackedScene>("res://scenes/bullet.tscn");
	private Marker3D _muzzle;
	
	private const float ProjectileVelocityMultiplier = 50;

	public override void _Ready()
	{
		_muzzle = GetNode<Marker3D>("Muzzle");
	}

	public void Shoot()
	{
		var bullet = _bulletScene.Instantiate<RigidBody3D>();
		bullet.Transform = _muzzle.GlobalTransform;
		bullet.LinearVelocity = -_muzzle.GlobalBasis.Y * ProjectileVelocityMultiplier;
		GetNode("/root/World").AddChild(bullet);
		// GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
	}
}