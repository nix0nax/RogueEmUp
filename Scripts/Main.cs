using Godot;
using System;
using System.Linq;
using System.Diagnostics;

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
	bool changeScene;
	AnimationPlayer animationToWaitFor;
	Random rng;
	int floorx = 640;
	int floorylow = 185;
	int flooryhi = 350;

	int numOfEnemies;
	public int playerHealth;
	public int highScore;

	int test = 1;
	public override void _Ready()
	{
		playerHealth = 100;
		rng = new Random();
		rootNode = this.GetTree().Root;
		currentScene = CurrentScene.Start;
		animationToWaitFor = this.GetNode("StartScreen").GetNode<AnimationPlayer>("AnimationPlayer");
		numOfEnemies = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (changeScene)
		{
			//Trace.WriteLine("yo");
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
					if (Input.IsActionJustPressed("Light"))
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
		Node scene = ResourceLoader.Load<PackedScene>("res://Scenes/Fight.tscn").Instantiate();
		scene.Name = "Fight";
		var player = ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn").Instantiate();
		player.Name = "Player";
		((Node2D)player).Position = new Vector2(320, 200);
		scene.AddChild(player);
		numOfEnemies = rng.Next(1,7);
		var numOfskeletons = rng.Next(1,4);
		var numOfEmeny = numOfEnemies - numOfskeletons;
		for (int i = 0; i < numOfEmeny; i++)
		{
			var emeny = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy.tscn").Instantiate();
			((Node2D)emeny).Position = new Vector2(rng.Next(floorx), rng.Next(floorylow, flooryhi));
			scene.AddChild(emeny);
		}

		for (int i = 0; i < numOfskeletons; i++)
		{
			var skeleton = ResourceLoader.Load<PackedScene>("res://Scenes/Skeleton.tscn").Instantiate();
			((Node2D)skeleton).Position = new Vector2(rng.Next(floorx), rng.Next(floorylow, flooryhi));
			scene.AddChild(skeleton);
		}

		
		rootNode.AddChild(scene);
	}
}
