using Godot;
using System;
using System.Diagnostics;

public partial class Fight : Node2D
{

	public int playerHealth;
	Node rootNode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rootNode = this.GetTree().Root;
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

	public void PlayerSetHealth(int setHealthTo)
	{
		var healthNode = this.GetNode<Sprite2D>("UI/Health");
		playerHealth = setHealthTo;
		if (playerHealth < 1)
		{
			healthNode.Frame = 62;
		}
		else if (playerHealth < 21)
		{
			healthNode.Frame = 61;
		}
		else if (playerHealth < 41)
		{
			healthNode.Frame = 60;
		}
		else if (playerHealth < 61)
		{
			healthNode.Frame = 59;
		}
		else if (playerHealth < 81)
		{
			healthNode.Frame = 58;
		}
		else
		{
			healthNode.Frame = 57;
		}
	}

	public void PlayerSetHighscore(int score){

		var scoreNode = (Label)this.GetNode<Label>("UI/HighScoreInt");
		scoreNode.Text = (scoreNode.Text.ToInt() + score).ToString();
		if(scoreNode.Text.ToInt() < 0){
			scoreNode.Text = "0";
		}
	}
}
