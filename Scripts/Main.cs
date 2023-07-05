using Godot;
using System;
using System.Linq;
using System.Diagnostics;

public partial class Main : Node2D
{
	public Node fightSceneNode;
	public Fight fightScene;
	public Node levelSelectSceneNode;
	public LevelSelect levelSelectScene;
	public StartScreen startScene;
	public Node rootNode;

	public override void _Ready()
	{
		
		rootNode = this.GetTree().Root;

		fightSceneNode = ResourceLoader.Load<PackedScene>("res://Scenes/Fight.tscn").Instantiate();
		levelSelectSceneNode = ResourceLoader.Load<PackedScene>("res://Scenes/LevelSelect.tscn").Instantiate();
		fightScene = (Fight)fightSceneNode;
		levelSelectScene = (LevelSelect)levelSelectSceneNode;
		fightScene.Hide();
		levelSelectScene.Hide();

		startScene = this.GetNode<StartScreen>("StartScreen");

		levelSelectScene.SelectHealOrUpgrade += SelectHealOrUpgrade;
		fightScene.FightEnded += () => 
		{
			rootNode.AddChild(levelSelectScene);
			levelSelectScene.Show();
			startScene.RemoveChild(fightScene);
			fightScene.Hide();
		};

		startScene.GameStarted += () => 
		{
			rootNode.AddChild(fightScene);
			fightScene.Show();
			startScene.Hide();
			startScene.Free();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void SelectHealOrUpgrade(bool healSelected)
	{
			rootNode.AddChild(fightScene);
			fightScene.Show();
			levelSelectScene.RemoveChild(levelSelectScene);
			levelSelectScene.Hide();
	}

	//public void FightExit()
	//{
	//	this.GetTree().Root.AddChild(levelSelectScene);
	//}
//
	//public void TreeExit()
	//{
	//	this.GetTree().Root.AddChild(fightScene);
	//}
}
