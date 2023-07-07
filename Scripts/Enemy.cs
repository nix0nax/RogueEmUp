using Godot;
using System;
using System.Diagnostics;

public partial class Enemy : Area2D
{
	public int speed = 6 ;
	public bool jumping;
	public bool attacking;
	public string directionString;
	public bool moving;
	public bool collidingWithTop;
	Vector2 velocity;
	Random rng;
	Node rootNode;
	int ct = 0;
	bool decided;
	Vector2 destination;
	Vector2 direction;
	int marginXLow = 25;
	int marginXHigh = 35;
	int marginY = 10;

	Player player;

	int health;

	public AnimatedSprite2D animatedSprite;
	public AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rootNode = this.GetTree().Root;
		player = (Player)rootNode.GetNode("Fight/Player");
		rng = new Random();
		velocity = new Vector2(0,0);

		decided = false;
		jumping = false;
		collidingWithTop = false;
		attacking = false;
		directionString = "_right";
		moving = false;
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");

		health = 100;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{

		// rnd = new Random();
		// Vector2 direction =  new Vector2((float)rnd.NextDouble() * rnd.NextInt64(-1,1), (float)rnd.NextDouble() * rnd.NextInt64(-1,1));
		// var button = Input.GetActionStrength("Punch");
		// if(button != 1){
		// 	animatedSprite.Animation = "Punch";
		// }


		if(!decided){
			//naj si zbere kaj bo glede na random
			long decision = rng.Next(1,3);

			switch(decision)
			{
				case 1:
					//	Idi do playerja pa ga vsipaj
					direction = (player.Position - this.Position).Normalized();
					destination = new Vector2(player.Position.X, player.Position.Y);
					break;
				case 2:
					//	Idi na random location	
					Vector2 rngVector = new Vector2(rng.Next(100,600),rng.Next(190,350));
					destination =  rngVector;
					direction = (destination - this.Position).Normalized();;
					break;
			}

			decided = true;
		}

		// DECISIONS	
		// naj gre do playerja
		// gre na random location
		// ko je dovol blizu ga naj vsipa

		var distance = destination - Position;
		if(Math.Abs(distance.X) < marginXHigh && Math.Abs(distance.Y) < marginXHigh){
			decided = false;
		}

		// (100-600,190-350)

		// ko pride kam more prit, decided = false

		if (!attacking)
		{
			float slowdown = 1F;
			if (direction.X != 0 && direction.Y != 0)
			{
				slowdown = 0.5F;
			}

			if (collidingWithTop && !jumping && direction.Y < 0)
			{
				direction.Y = 0;
			}
			
			velocity.X = direction.X * speed * slowdown;
			velocity.Y = direction.Y * speed* slowdown;

			if (direction == Vector2.Zero)
			{
				moving = false;
			}
			else
			{
				moving = true;
			}

			if (direction.X < 0)
			{
				directionString = "_left";
			}
			else if (direction.X > 0)
			{
				directionString = "_right";
			}
		}

		if (attacking && !animationPlayer.IsPlaying())
		{
			attacking = false;
		}

		var distanceToPlayer = player.Position - Position;
		// Attacks go here
		if (!attacking && Math.Abs(distanceToPlayer.X) < marginXHigh && Math.Abs(distanceToPlayer.X) > marginXLow && Math.Abs(distanceToPlayer.Y) < marginY)
		{
			var attackType = string.Empty;
			attacking = true;
			moving = false;
			velocity.X = 0;
			velocity.Y = 0;
			
			long decision = rng.Next(1,2);

			if (distanceToPlayer.X > 0)
			{
				directionString = "_right";
			}
			else
			{
				directionString = "_left";
			}

			switch(decision)
			{
				case 1:
					attackType = "heavy";
					break;
			}

			if (!string.IsNullOrEmpty(attackType))
			{
				animationPlayer.Play($"{attackType}{directionString}");
			}
		};

		// animate if not atttacking
		if (!attacking)
		{
			if (moving)
			{
				animationPlayer.Play($"run{directionString}");
			}
			else
			{
				animationPlayer.Play($"idle{directionString}");
			}
		}
		
		this.Position += velocity;
	}

	//private void UpdateAnimationParameters(){
	//	animationTree.Set("parameters/isIdle",true);
//
	//}

	private void OnPunchHitAreaEntered(Area2D area)
	{	
		if(area.IsInGroup(new StringName("HurtBox"))){
			
		}
	}

	private void HeavyHit(Node2D node)
	{
		if (node.GetType() == typeof(PlantColission))
		{
			((PlantColission)node).TakeDamage(10);
		}
	}

	public void TakeDamage(int damage)
	{
		health -= damage;
	}
}



