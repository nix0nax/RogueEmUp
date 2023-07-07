using Godot;
using System;

public partial class EnemyHitbox : Area2D
{
	Enemy parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = (Enemy)this.GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TakeDamage(int damage)
	{
		parent.TakeDamage(damage);
	}
}
