using Godot;
using System;

public partial class LevelSelect : Control
{
	AnimationPlayer animationPlayer;
	bool selected;
	public bool healSelected;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("HealBob");

		healSelected = true;
		//SelectHealOrUpgrade += SelectHealOrUpgrade;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Right"))
		{
			animationPlayer.Play("UpgradeBob");
			healSelected = false;
		}

		if (Input.IsActionJustPressed("Left"))
		{
			animationPlayer.Play("HealBob");
			healSelected = true;
		}

		if (Input.IsActionJustPressed("Light"))
		{
			if (healSelected)
			{
				animationPlayer.Play("HealSelect");
			}
			else
			{
				animationPlayer.Play("UpgradeSelect");
			}
		}
	}
}
