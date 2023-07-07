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


	int test = 1;
	public override void _Ready()
	{
		rootNode = this.GetTree().Root;
		currentScene = CurrentScene.Start;
		animationToWaitFor = this.GetNode("StartScreen").GetNode<AnimationPlayer>("AnimationPlayer");
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
						scene = ResourceLoader.Load<PackedScene>("res://Scenes/Fight.tscn").Instantiate();
						scene.Name = "Fight";
						var player1 = ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn").Instantiate();
						((Node2D)player1).Position = new Vector2(320, 180);
						scene.AddChild(player1);
						var plant1 = ResourceLoader.Load<PackedScene>("res://Scenes/Plant.tscn").Instantiate();
						((Node2D)plant1).Position = new Vector2(120, 180);
						scene.AddChild(plant1);
						currentScene = CurrentScene.Fight;
						break;
					case CurrentScene.LevelSelect:
						remove = rootNode.GetNode("LevelSelect");
						scene = ResourceLoader.Load<PackedScene>("res://Scenes/Fight.tscn").Instantiate();
						scene.Name = "Fight";
						var player2 = ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn").Instantiate();
						player2.Name = "Player";
						((Node2D)player2).Position = new Vector2(320, 200);
						scene.AddChild(player2);
						var plant2 = ResourceLoader.Load<PackedScene>("res://Scenes/Plant.tscn").Instantiate();
						((Node2D)plant2).Position = new Vector2(120, 200);
						scene.AddChild(plant2);
						var emeny = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy.tscn").Instantiate();
						((Node2D)emeny).Position = new Vector2(400, 200);
						scene.AddChild(emeny);
						var skeleton1 = ResourceLoader.Load<PackedScene>("res://Scenes/Skeleton.tscn").Instantiate();
						((Node2D)skeleton1).Position = new Vector2(520, 200);
						scene.AddChild(skeleton1);
						currentScene = CurrentScene.Fight;
						break;
					case CurrentScene.Fight:
						remove = rootNode.GetNode("Fight");
						scene = ResourceLoader.Load<PackedScene>("res://Scenes/LevelSelect.tscn").Instantiate();
						scene.Name = "LevelSelect";
						
						currentScene = CurrentScene.LevelSelect;
						break;
				}

				rootNode.AddChild(scene);
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
}
