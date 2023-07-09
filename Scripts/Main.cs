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
		UpgradeSelect,
	}

	CurrentScene currentScene;
	CurrentScene goToScene;
	bool changeScene;
	AnimationPlayer animationToWaitFor;
	Random rng;
	int floorx = 640;
	int floorylow = 185;
	int flooryhi = 350;

	int level;

	public int numOfEnemies;
	public int playerHealth;
	public int highScore;

	List<Upgrade> playerUpgrade;
	bool healSelected = false;

	public List<Enemy> enemies;

	int test = 1;
	public override void _Ready()
	{
		enemies = new List<Enemy>();
		playerHealth = 100;
		rng = new Random();
		rootNode = this.GetTree().Root;
		currentScene = CurrentScene.Start;
		goToScene = CurrentScene.Fight;
		animationToWaitFor = this.GetNode("StartScreen").GetNode<AnimationPlayer>("AnimationPlayer");
		numOfEnemies = 0;
		playerUpgrade = new List<Upgrade>();
		level = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (changeScene)
		{
			if (animationToWaitFor == null || (!animationToWaitFor.IsPlaying() && animationToWaitFor.CurrentAnimationLength == animationToWaitFor.CurrentAnimationPosition))
			{
				Node scene = null;
				Node remove = null;
				switch (currentScene)
				{
					case CurrentScene.Start:
						remove = this.GetNode("StartScreen");
						break;
					case CurrentScene.LevelSelect:
						remove = rootNode.GetNode("LevelSelect");
						break;
					case CurrentScene.Fight:
						remove = rootNode.GetNode("Fight");
						break;
					case CurrentScene.UpgradeSelect:
						remove = rootNode.GetNode("UpgradeSelect");
						break;
				}

				remove.Free();

				switch (goToScene)
				{
					case CurrentScene.Fight:
						this.GenerateFight();
						currentScene = CurrentScene.Fight;
						break;
					case CurrentScene.LevelSelect:
						scene = ResourceLoader.Load<PackedScene>("res://Scenes/LevelSelect.tscn").Instantiate();
						scene.Name = "LevelSelect";
						rootNode.AddChild(scene);
						currentScene = CurrentScene.LevelSelect;
						break;
					case CurrentScene.UpgradeSelect:
						scene = ResourceLoader.Load<PackedScene>("res://Scenes/UpgradeSelect.tscn").Instantiate();
						scene.Name = "UpgradeSelect";
						rootNode.AddChild(scene);
						currentScene = CurrentScene.UpgradeSelect;
						break;
				}

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
						goToScene = CurrentScene.Fight;
						changeScene = true;
						animationToWaitFor = this.GetNode("StartScreen").GetNode<AnimationPlayer>("AnimationPlayer");
					}
					break;
				case CurrentScene.LevelSelect:
					if (Input.IsActionJustPressed("Light"))
					{
						var levelNode = (LevelSelect)rootNode.GetNode("LevelSelect");
						if (levelNode.healSelected)
						{
							healSelected = true;
							goToScene = CurrentScene.Fight;
						}
						else
						{
							healSelected = false;
							goToScene = CurrentScene.UpgradeSelect;
						}
						animationToWaitFor = levelNode.GetNode<AnimationPlayer>("AnimationPlayer");
						changeScene = true;
					}
					break;
				case CurrentScene.Fight:
					if (numOfEnemies < 1)
					{
						goToScene = CurrentScene.LevelSelect;
						changeScene = true;
						animationToWaitFor = null;
					}
					else if (playerHealth <= 0 && Input.IsActionJustPressed("Light"))
					{
						goToScene = CurrentScene.Fight;
						playerUpgrade.Clear();
						playerHealth = 100;
						changeScene = true;
						level = 1;
						//animationToWaitFor = rootNode.GetNode("Fight/Player").GetNode<AnimationPlayer>("AnimationPlayer");
						animationToWaitFor = null;
					}
					break;
				case CurrentScene.UpgradeSelect:
					if (Input.IsActionJustPressed("Light"))
					{
						goToScene = CurrentScene.Fight;
						changeScene = true;
						animationToWaitFor = rootNode.GetNode("UpgradeSelect").GetNode<AnimationPlayer>("AnimationPlayer");
					}
					break;
			}
		}
		
		
	}

	public void GenerateFight()
	{
		// Clear damage timers you have to set for stagger effect on hit
		enemies.Clear();

		// Create Fight scene
		Node scene = ResourceLoader.Load<PackedScene>("res://Scenes/Fight.tscn").Instantiate();
		scene.Name = "Fight";

		// Make player
		var player = GeneratePlayer((Fight)scene);
		scene.AddChild(player);
		

		// get total number of enemies to spawn and choose how many of which
		numOfEnemies = rng.Next(1 * level,2 * level);
		//var numOfskeletons = rng.Next(1,4);
		var numOfEmeny = numOfEnemies;// - numOfskeletons;

		// Spawn emeny enemy
		for (int i = 0; i < numOfEmeny; i++)
		{
			Enemy emeny = (Enemy)ResourceLoader.Load<PackedScene>("res://Scenes/Enemy.tscn").Instantiate();
			((Node2D)emeny).Position = new Vector2(rng.Next(floorx), rng.Next(floorylow, flooryhi));
			emeny.heavyDamage = 10 + 5 * rng.Next(0, level/2);
			emeny.heavyAttackSpeed = 1F + 0.2F * rng.Next(0, level/2);
			emeny.speed = 4 + rng.Next(0, level/2);
			emeny.health = 50 + 10 * + rng.Next(0, level);
			scene.AddChild(emeny);
			enemies.Add(emeny);
		}

		((Fight)scene).SetLevel(level);
		level++;

        // // Spawn skeleton enemey
        // for (int i = 0; i < numOfskeletons; i++)
        // {
        //     var skeleton = ResourceLoader.Load<PackedScene>("res://Scenes/Skeleton.tscn").Instantiate();
        //     ((Node2D)skeleton).Position = new Vector2(rng.Next(floorx), rng.Next(floorylow, flooryhi));
        //     scene.AddChild(skeleton);
        // }

		
		rootNode.AddChild(scene);
	}

	public Player GeneratePlayer(Fight fightScene)
	{
		var player = (Player)ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn").Instantiate();
		player.Name = "Player";
		((Node2D)player).Position = new Vector2(320, 200);

		player.heavyDamage = 15;
		player.lightDamage = 5;
		player.heavyAttackSpeed = 1F;
		player.speed = 4;
		player.maxHealth = 100;

		// upgrades
		foreach (var upgrade in playerUpgrade)
		{
			switch(upgrade.UpgradeType)
			{
				case "Damage":
					player.heavyDamage += 5 * upgrade.Amount;
					player.lightDamage += 5 * upgrade.Amount;
					break;
				case "AttackSpeed":
					player.heavyAttackSpeed += 0.2F * upgrade.Amount;
					break;
				case "MaxHp":
					player.maxHealth += 50 * upgrade.Amount;
					break;
				case "PlayerSpeed":
					player.speed += 2 * upgrade.Amount;
					break;
			}
		}

		if (healSelected)
		{
			playerHealth = player.maxHealth;
		}

		player.health = playerHealth;
		fightScene.playerHealth = playerHealth;
		fightScene.playerMaxHealth = player.maxHealth;

		return (Player)player;
	}

	public void HitOccured(float time)
	{
		var playerTimer = rootNode.GetNode<Timer>("Fight/Player/DamageTimer");
		playerTimer.Start(time);
		foreach (var enemy in enemies)
		{
			var timer = enemy.GetNode<Timer>("DamageTimer");
			timer.Start(time);
		}
	}

	public void PlayerUpgrade(string upgradeToAdd)
	{
		var upgrade = playerUpgrade.FirstOrDefault(x => x.UpgradeType == upgradeToAdd);
		if (upgrade == null)
		{
			playerUpgrade.Add(new Upgrade
			{
				UpgradeType = upgradeToAdd,
				Amount = 1,
			});
		}
		else
		{
			upgrade.Amount++;
		}

		if (upgradeToAdd == "MaxHp")
		{
			playerHealth += 50;
		}
	}
}

public class Upgrade
{
	public string UpgradeType {get;set;}
	public int Amount {get;set;}
}
