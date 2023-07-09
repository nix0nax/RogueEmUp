using Godot;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

public partial class Main : Node2D
{
	Node rootNode;
	Node mainScene;
	enum CurrentScene
	{
		Start,
		Fight,
		LevelSelect,
	}

	CurrentScene currentScene;
	CurrentScene goToScene;
	bool changeScene;
	AnimationPlayer animationToWaitFor;
	Random rng;
	int floorx = 640;
	int floorylow = 185;
	int flooryhi = 350;

	public int numOfEnemies;
	public int playerHealth;
	public int highScore;

	List<Timer> damageTimersToStart;

	int test = 1;
	public override void _Ready()
	{
		damageTimersToStart = new List<Timer>();
		playerHealth = 100;
		rng = new Random();
		rootNode = this.GetTree().Root;
		currentScene = CurrentScene.Start;
		goToScene = CurrentScene.Fight;
		animationToWaitFor = this.GetNode("StartScreen").GetNode<AnimationPlayer>("AnimationPlayer");
		numOfEnemies = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (changeScene)
		{
			if (animationToWaitFor == null || !animationToWaitFor.IsPlaying())
			{
				Node scene = null;
				Node remove = null;
				switch (currentScene)
				{
					case CurrentScene.Start:
						remove = this.GetNode("StartScreen");
						this.GenerateFight();
						currentScene = CurrentScene.Fight;
						break;
					case CurrentScene.LevelSelect:
						remove = rootNode.GetNode("LevelSelect");
						this.GenerateFight();
						currentScene = CurrentScene.Fight;
						break;
					case CurrentScene.Fight:
						remove = rootNode.GetNode("Fight");
						scene = ResourceLoader.Load<PackedScene>("res://Scenes/LevelSelect.tscn").Instantiate();
						scene.Name = "LevelSelect";
						rootNode.AddChild(scene);
						
						currentScene = CurrentScene.LevelSelect;
						break;
				}

				remove.QueueFree();
				changeScene = false;
			}
		}
		else
		{
			switch (currentScene)
			{
				case CurrentScene.Start:
					if (Input.IsActionJustPressed("Light"))
					{
						changeScene = true;
						animationToWaitFor = this.GetNode("StartScreen").GetNode<AnimationPlayer>("AnimationPlayer");
					}
					break;
				case CurrentScene.LevelSelect:
					if (Input.IsActionJustPressed("Light"))
					{
						changeScene = true;
						animationToWaitFor = rootNode.GetNode("LevelSelect").GetNode<AnimationPlayer>("AnimationPlayer");
					}
					break;
				case CurrentScene.Fight:
					if (Input.IsActionJustPressed("Special"))
					{
						changeScene = true;
						animationToWaitFor = null;
					}
					break;
			}
		}
		
		
	}

	public void GenerateFight()
	{
		// Clear damage timers you have to set for stagger effect on hit
		damageTimersToStart.Clear();

		// Create Fight scene
		Node scene = ResourceLoader.Load<PackedScene>("res://Scenes/Fight.tscn").Instantiate();
		scene.Name = "Fight";

		// Make player
		var player = ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn").Instantiate();
		player.Name = "Player";
		((Node2D)player).Position = new Vector2(320, 200);
		scene.AddChild(player);
		damageTimersToStart.Add(player.GetNode<Timer>("DamageTimer"));

		// get total number of enemies to spawn and choose how many of which
		numOfEnemies = rng.Next(1,7);
		var numOfskeletons = rng.Next(1,4);
		var numOfEmeny = numOfEnemies - numOfskeletons;

		// Spawn emeny enemy
		for (int i = 0; i < numOfEmeny; i++)
		{
			var emeny = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy.tscn").Instantiate();
			((Node2D)emeny).Position = new Vector2(rng.Next(floorx), rng.Next(floorylow, flooryhi));
			scene.AddChild(emeny);
			damageTimersToStart.Add(emeny.GetNode<Timer>("DamageTimer"));
		}


		// Spawn skeleton enemey
		for (int i = 0; i < numOfskeletons; i++)
		{
			var skeleton = ResourceLoader.Load<PackedScene>("res://Scenes/Skeleton.tscn").Instantiate();
			((Node2D)skeleton).Position = new Vector2(rng.Next(floorx), rng.Next(floorylow, flooryhi));
			scene.AddChild(skeleton);
		}

		
		rootNode.AddChild(scene);
	}

	public void HitOccured(float time)
	{
		foreach (var timer in damageTimersToStart)
		{
			timer.Start(time);
		}
	}
}
