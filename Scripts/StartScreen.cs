using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class StartScreen : Control
{
	AnimationPlayer animationPlayer;
	bool aboutToStart;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("TextBob");

		aboutToStart = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Light"))
		{
			animationPlayer.Play("Start");
			aboutToStart = true;
			//this.GetTree().ChangeSceneToFile("res://Scenes/main_scene.tscn");
		}

		if (Input.IsActionJustPressed("Escape"))
		{
			this.GetTree().Quit();
		}
	}
}
