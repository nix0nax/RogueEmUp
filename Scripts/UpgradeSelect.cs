using Godot;
using System;

public partial class UpgradeSelect : Control
{
	AnimationPlayer animationPlayer;
	bool selected;
	bool leftSelected;
	Sprite2D left;
	Sprite2D right;
	string leftCode;
	string rightCode;
	string leftLabel;
	string rightLabel;

	Label label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = this.GetNode<Label>("Label");

		Random rng = new Random();
		left = this.GetNode<Sprite2D>("Upgrade1");
		right = this.GetNode<Sprite2D>("Upgrade2");
		var firstResult = rng.Next(1,5);
		var secondResult = rng.Next(1,5);
		while (firstResult == secondResult)
		{
			secondResult = rng.Next(1,4);
		}

		switch(firstResult)
		{
			case 1:
				leftCode = "Damage";
				left.Frame = 30;
				leftLabel = "Increase attack damage";
				break;
				
			case 2:
				leftCode = "AttackSpeed";
				left.Frame = 8;
				leftLabel = "Increase heavy attack speed";
				break;
				
			case 3:
				leftCode = "MaxHp";
				left.Frame = 16;
				leftLabel = "Increase max health";
				break;
				
			case 4:
				leftCode = "PlayerSpeed";
				left.Frame = 11;
				leftLabel = "Increase movement speed";
				break;
		}

		switch(secondResult)
		{
			case 1:
				rightCode = "Damage";
				right.Frame = 30;
				rightLabel = "Increase attack damage";
				break;
				
			case 2:
				rightCode = "AttackSpeed";
				right.Frame = 8;
				rightLabel = "Increase heavy attack speed";
				break;
				
			case 3:
				rightCode = "MaxHp";
				right.Frame = 16;
				rightLabel = "Increase max health";
				break;
				
			case 4:
				rightCode = "PlayerSpeed";
				right.Frame = 11;
				rightLabel = "Increase movement speed";
				break;
		}
		
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("LeftBob");
		label.Text = leftLabel;

		leftSelected = true;
		//SelectHealOrUpgrade += SelectHealOrUpgrade;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Right"))
		{
			animationPlayer.Play("RightBob");
			label.Text = rightLabel;
			leftSelected = false;
		}

		if (Input.IsActionJustPressed("Left"))
		{
			animationPlayer.Play("LeftBob");
			label.Text = leftLabel;
			leftSelected = true;
		}

		if (Input.IsActionJustPressed("Light") && !selected)
		{
			Main mainNode = this.GetTree().Root.GetNode<Main>("Main");
			selected = true;
			if (leftSelected)
			{
				animationPlayer.Play("LeftSelect");
				mainNode.PlayerUpgrade(leftCode);
			}
			else
			{
				animationPlayer.Play("RightSelect");
				mainNode.PlayerUpgrade(rightCode);
			}
		}
	}
}
