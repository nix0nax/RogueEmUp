using Godot;
using System;
using System.Diagnostics;

public partial class Plant : Node2D
{
	int hp;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hp = 100;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TakeDamage(int damage)
	{
		hp -= damage;
		Trace.WriteLine(hp);
	}
}
