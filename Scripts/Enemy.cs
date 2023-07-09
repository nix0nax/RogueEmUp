using Godot;
using System;
using System.Diagnostics;

public partial class Enemy : Area2D
{
	public int speed = 4;
	public float heavyAttackSpeed;
	public int heavyDamage = 10;
	public float heavyDamageTimer = 0.4F;
	public bool thinking;
	public bool damagePaused;
	public bool hurt;
	public bool jumping;
	public bool attacking;
	public string directionString;
	public bool moving;
	public bool collidingWithTop;
	public bool chasingPlayer = false;
	public int directionPlayer = -1;
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
	ulong playerId;
	public Timer damageTimer;
	public Timer thinkingTimer;

	public int health = 50;

	public AnimatedSprite2D animatedSprite;
	public AnimationPlayer animationPlayer;
	Main mainNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rootNode = this.GetTree().Root;
		mainNode = rootNode.GetNode<Main>("Main");

		player = (Player)rootNode.GetNode("Fight/Player");
		playerId = player.GetInstanceId();
		rng = new Random();
		velocity = new Vector2(0,0);

		// Get DamageTimer and set event to it
		damagePaused = false;
		damageTimer = this.GetNode<Timer>("DamageTimer");
		damageTimer.Timeout += () => damagePaused = false;

		
		// Get ThinkingTimer and set event to it
		thinking = true;
		thinkingTimer = this.GetNode<Timer>("ThinkingTimer");
		thinkingTimer.Timeout += () => thinking = false;
		thinkingTimer.Start(0.5);

		hurt = false;
		decided = false;
		jumping = false;
		collidingWithTop = false;
		attacking = false;
		directionString = "_right";
		moving = false;
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (hurt && !animationPlayer.IsPlaying() && animationPlayer.CurrentAnimationLength == animationPlayer.CurrentAnimationPosition)
		{
			hurt = false;
			if (health < 0)
			{
				Main mainNode = (Main)rootNode.GetNode("Main");
				mainNode.numOfEnemies -= 1;
				mainNode.enemies.Remove(this);

				this.QueueFree();
			}
		}

		// playing hurt animation
		if (hurt)
		{
			return;
		}

		if (((Fight)this.GetParent()).gameOver)
		{
			chasingPlayer = false;
		}

		// If paused, literally do nothing 
		if (damagePaused)
		{
			return;
		}

		if(!decided){
			//naj si zbere kaj bo glede na random
			long decision = rng.Next(0,101);

			if (decision < 50 && !((Fight)this.GetParent()).gameOver)
			{
					//	Idi do playerja pa ga vsipaj
					directionPlayer = rng.Next(0,2) == 1 ? -1 : 1;
					chasingPlayer = true;
			}
			else
			{
					//	Idi na random location	
					Vector2 rngVector = new Vector2(rng.Next(100,600),rng.Next(190,350));
					destination =  rngVector;
					direction = (destination - this.Position).Normalized();;

			}

			decided = true;
		}

		if (thinking)
		{
			if (!hurt)
			{
				animationPlayer.Play($"idle{directionString}");
			}
			return;
		}

		// if chasing player, update where to go every frame
		if (chasingPlayer)
		{
			destination = new Vector2(player.Position.X + (25 * directionPlayer), player.Position.Y);
			direction = (destination - this.Position).Normalized();
		}

		// DECISIONS	
		// naj gre do playerja
		// gre na random location
		// ko je dovol blizu ga naj vsipa

		var distance = destination - Position;
		if(Math.Abs(distance.X) < marginXLow && Math.Abs(distance.Y) < marginY && !attacking)
		{
			decided = false;
			chasingPlayer = false;
			thinking = true;
			float nextFloat = (float)rng.NextDouble();
			thinkingTimer.Start(nextFloat);
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

		if (attacking && !animationPlayer.IsPlaying() &&  animationPlayer.CurrentAnimationLength == animationPlayer.CurrentAnimationPosition)
		{
			attacking = false;
		}

		var distanceToPlayer = ((Fight)this.GetParent()).gameOver ? Vector2.Zero : player.Position - Position;
		// Attacks go here
		if (!attacking && Math.Abs(distanceToPlayer.X) < marginXHigh && Math.Abs(distanceToPlayer.X) > marginXLow && Math.Abs(distanceToPlayer.Y) < marginY && !thinking)
		{
			var attackType = string.Empty;
			attacking = true;
			moving = false;
			velocity.X = 0;
			velocity.Y = 0;

			// make new decision after attack
			decided = false;
			chasingPlayer = false;
			
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
				animationPlayer.Play($"{attackType}{directionString}", customSpeed: heavyAttackSpeed);
			}
		};

		// animate if not atttacking
		if (!attacking && !hurt)
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

	private void HeavyHit(Node2D node)
	{
		if (node.GetType() == typeof(PlayerHitbox))
		{
			PlayerHitbox playerNode = (PlayerHitbox)node;
			playerNode.TakeDamage(heavyDamage, ((Player)playerNode.GetParent()).Position.X < this.Position.X ? true : false);
			mainNode.HitOccured(heavyDamageTimer);
		}
	}

	public void TakeDamage(int damage, bool facingRight)
	{
		health -= damage;
		hurt = true;
		directionString = facingRight ? "_right" : "_left";
		attacking = false;
		if (health < 0)
		{
			animationPlayer.Play($"death{directionString}");
		}
		else
		{
			animationPlayer.Play($"hurt{directionString}");
		}
	}

	public void StartDamageTimer(float time)
	{
		damagePaused = true;
		attacking = false;
		decided = false;
		chasingPlayer = false;
		damageTimer.Start(time);
	}
}



