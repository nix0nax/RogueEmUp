using Godot;
using System;

public partial class PlayerHitbox : Area2D
{
	Player parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = (Player)this.GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TakeDamage(int damage, bool facingRight)
	{
		parent.TakeDamage(damage, facingRight);
	}
}
