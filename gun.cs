using Godot;
using System;

public partial class gun : MeshInstance3D
{
	PackedScene BulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void Shoot()
	{
		// Create a new bullet instance
		PhysicsBody3D bullet = BulletScene.Instantiate() as PhysicsBody3D;
		// Add the bullet to the scene
		GetParent().AddChild(bullet);
		// Set the bullet's position to the gun's position
		bullet.GlobalTransform = GlobalTransform;
		// Play the shooting sound
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
	}
}
