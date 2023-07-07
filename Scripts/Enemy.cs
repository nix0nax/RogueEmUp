using Godot;
using System;
using System.Diagnostics;

public partial class Enemy : AnimatableBody2D
{
	public int speed ;
	public bool jumping;
	public bool attacking;
	public string directionString;
	public bool moving;
	public bool collidingWithTop;
	Vector2 velocity;
	Random rng;
	Node rootNode;
	int ct = 0;
	bool decided = false;


	public AnimatedSprite2D animatedSprite;
	public AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		rootNode = this.GetTree().Root;
		speed = 6;
		jumping = false;
		collidingWithTop = false;
		attacking = false;
		directionString = "_right";
		moving = false;
		velocity = new Vector2(0,0);
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		rng = new Random();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		// rnd = new Random();
		// Vector2 direction =  new Vector2((float)rnd.NextDouble() * rnd.NextInt64(-1,1), (float)rnd.NextDouble() * rnd.NextInt64(-1,1));
		// var button = Input.GetActionStrength("Punch");
		// if(button != 1){
		// 	animatedSprite.Animation = "Punch";
		// }

		var Player = (Player)rootNode.GetNode("Fight/Player");
		Vector2 destination = new Vector2(0,0);
		Vector2 direction = new Vector2(0,0);

		if(!decided){
			//naj si zbere kaj bo glede na random
			long decision = rng.Next(1,3);
			if(decision == 1){
				direction = (Player.Position - this.Position).Normalized();
				destination = (Player.Position - this.Position).Normalized();
				decided = true;
			}
				//	Idi do playerja pa ga vsipaj
			if(decision == 2){
				Vector2 rngVector = new Vector2(rng.Next(100,600),rng.Next(190-350));
				direction = rngVector;
				destination = rngVector;
				decided = true;
			}
				//	Idi na random location			
		}


		// DECISIONS	
		// naj gre do playerja
		// gre na random location
		// ko je dovol blizu ga naj vsipa


		if(!(Math.Abs(Player.Position.X - this.Position.X) > 25 || Math.Abs(Player.Position.Y - this.Position.Y) > 25) ){
			ct++;
			Trace.WriteLine($"yee {ct}");
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

		// // Attacks go here
		// if (!attacking && Input.IsActionJustPressed("Attack"))
		// {
		// 	var attackType = string.Empty;
		// 	attacking = true;
		// 	moving = false;
		// 	velocity.X = 0;
		// 	velocity.Y = 0;

		// 	if (Input.IsActionJustPressed("Heavy"))
		// 	{
		// 		attackType = "heavy";
		// 	}
		// 	//else if (Input.IsActionJustPressed("Light"))
		// 	//{
		// 	//	attackType = "light";
		// 	//}
		// 	else if (Input.IsActionJustPressed("Special"))
		// 	{
		// 		attackType = "special";
		// 	}

		// 	if (!string.IsNullOrEmpty(attackType))
		// 	{
		// 		animationPlayer.Play($"{attackType}{directionString}");
		// 	}
		// };

		// // animate if not atttacking
		// if (!attacking)
		// {
		// 	if (moving)
		// 	{
		// 		animationPlayer.Play($"run{directionString}");
		// 	}
		// 	else
		// 	{
		// 		animationPlayer.Play($"idle{directionString}");
		// 	}
		// }
		
		MoveAndCollide(velocity);
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
		//Trace.WriteLine(node.GetType());
		if (node.GetType() == typeof(PlantColission))
		{
			((PlantColission)node).TakeDamage(10);
		}
	}
}



