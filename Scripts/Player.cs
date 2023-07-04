using Godot;
using System;
using System.Diagnostics;

public partial class Player : AnimatableBody2D
{
	public int speed ;
	public bool jumping;
	public bool collidingWithTop;

	public AnimatedSprite2D animatedSprite;
	public AnimationTree animationTree;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		speed = 6;
		jumping = false;
		collidingWithTop = false;
		animationTree = GetNode<AnimationTree>("AnimationTree2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		animationTree.Active = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = ConstantLinearVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");

		// var button = Input.GetActionStrength("Punch");
		// if(button != 1){
		// 	animatedSprite.Animation = "Punch";
		// }

		Animate(direction);
		if (direction != Vector2.Zero)
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
		}

		//ConstantLinearVelocity = velocity;
		MoveAndCollide(velocity);


	}

	private void OnEnteredTopCollider(Node2D body)
	{
		if (this == body)
		{
			collidingWithTop = true;
		}
	}

	private void OnExitedTopCollider(Node2D body)
	{
		if (this == body)
		{
			collidingWithTop = false;
		}
	}

	private void Animate(Vector2 direction)
	{

	}

	private void UpdateAnimationParameters(){
		animationTree.Set("parameters/isIdle",true);

	}

	private void OnPunchHitAreaEntered(Area2D area)
	{	
		if(area.IsInGroup(new StringName("HurtBox"))){
			
		}
	}
}



