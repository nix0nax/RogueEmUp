using Godot;
using System;

public partial class Fight : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TopEnter(Node hit)
	{
		if (hit.GetType() == typeof(Player))
		{
			((Player)hit).collidingWithTop = true;
		}
	}

	public void TopExit(Node hit)
	{
		if (hit.GetType() == typeof(Player))
		{
			((Player)hit).collidingWithTop = false;
		}
	}
}
