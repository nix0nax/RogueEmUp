using Godot;
using System;
using System.Diagnostics;

public partial class Player : AnimatableBody2D
{
	public int maxHealth;
	public int health;
	public int speed = 4;
	public int heavyDamage;
	public float heavyDamageTimer = 0.4F;
	public float heavyAttackSpeed;
	public int lightDamage;
	public float lightDamageTimer = 0.1F;

	public bool canComboTimer;
	public bool canComboInput;
	public bool damagePaused;
	
	public bool jumping;
	public bool attacking;
	public string directionString;
	public bool moving;
	public bool hurt;
	public bool collidingWithTop;
	Vector2 velocity;

	// Get any nodes needed for logic
	public AnimatedSprite2D animatedSprite;
	public AnimationPlayer animationPlayer;
	Timer comboTimer;
	Node rootNode;
	Main mainNode;
	Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create combo timer (I could have done most of this this outside of code lmao)
		comboTimer = new Timer();
		comboTimer.OneShot = true;
		comboTimer.Timeout += () =>
		{
			canComboTimer = false;
		};
		canComboTimer = false;
		canComboInput = true;
		this.AddChild(comboTimer);

		damagePaused = false;

		rootNode = this.GetTree().Root;
		mainNode = rootNode.GetNode<Main>("Main");
		jumping = false;
		hurt = false;
		collidingWithTop = false;
		attacking = false;
		directionString = "_right";
		moving = false;
		velocity = new Vector2(0,0);
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		((Fight)rootNode.GetNode("Fight")).PlayerSetHighscore(0);
		player = (Player)rootNode.GetNode("Fight/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{

		if (hurt && !animationPlayer.IsPlaying()  && animationPlayer.CurrentAnimationLength == animationPlayer.CurrentAnimationPosition)
		{
			hurt = false;
		}

		// no spamming allowed >:(
		if (attacking && !canComboTimer && Input.IsActionJustPressed("Attack"))
		{
			canComboInput = false;
		}

		// If paused and can't combo, literally do nothing
		if ((!canComboTimer || !canComboInput) && damagePaused)
		{
			return;
		}

		// process attacks FIRST (this is to allow attacking during damage pause)
		if (attacking && !damagePaused && !animationPlayer.IsPlaying()  && animationPlayer.CurrentAnimationLength == animationPlayer.CurrentAnimationPosition)
		{
			attacking = false;
			canComboInput = true;
		}
		
		// Attacks go here
		if ((!attacking || (canComboTimer && canComboInput)) && Input.IsActionJustPressed("Attack"))
		{
			var attackType = string.Empty;
			var attackSpeed = 1F;
			attacking = true;
			canComboTimer = false;
			moving = false;
			velocity.X = 0;
			velocity.Y = 0;

			if (Input.IsActionJustPressed("Heavy"))
			{
				attackType = "heavy";
			}
			else if (Input.IsActionJustPressed("Light"))
			{
				attackType = "light";
			}
			else if (Input.IsActionJustPressed("Special"))
			{
				//attackType = "special";
				attackType = "heavy";
				attackSpeed = heavyAttackSpeed;
			}

			if (!string.IsNullOrEmpty(attackType))
			{
				animationPlayer.Stop();
				animationPlayer.Play($"{attackType}{directionString}", customSpeed: attackSpeed);
			}
		};

		// If paused, literally do nothing after getting punch inputs
		if (damagePaused)
		{
			return;
		}

		// Movement
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");

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

		// animate if not atttacking or hurt
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
		
		MoveAndCollide(velocity);
	}

	private void HeavyHit(Area2D node)
	{
		if (node.GetType() == typeof(EnemyHitbox))
		{
			EnemyHitbox enemyNode = (EnemyHitbox)node;
			enemyNode.TakeDamage(heavyDamage, enemyNode.Position.X < this.Position.X ? true : false);
			comboTimer.Start(0.2);
			canComboTimer = true;
			mainNode.HitOccured(heavyDamageTimer);
			((Fight)rootNode.GetNode("Fight")).PlayerSetHighscore(10);
		}

		if (node.GetType() == typeof(PlantColission))
		{
			((PlantColission)node).TakeDamage(heavyDamage);
			comboTimer.Start(0.2);
			canComboTimer = true;
			mainNode.HitOccured(heavyDamageTimer);
		}
	}

	private void LightHit(Area2D node)
	{
		if (node.GetType() == typeof(EnemyHitbox))
		{
			EnemyHitbox enemyNode = (EnemyHitbox)node;
			enemyNode.TakeDamage(heavyDamage, ((Enemy)enemyNode.GetParent()).Position.X < this.Position.X ? true : false);
			((Enemy)enemyNode.GetParent()).damagePaused = true;
			((Enemy)enemyNode.GetParent()).damageTimer.Start(lightDamageTimer);
			((Fight)rootNode.GetNode("Fight")).PlayerSetHighscore(10);
		}

		if (node.GetType() == typeof(PlantColission))
		{
			((PlantColission)node).TakeDamage(lightDamage);
			mainNode.HitOccured(lightDamageTimer);
		}
	}

	public void TakeDamage(int damage, bool facingRight)
	{
		// Ouch oof owie ouch I'm calling the take damage function
		health -= damage;
		mainNode.playerHealth = health;
		((Fight)rootNode.GetNode("Fight")).PlayerSetHealth(health);
		hurt = true;
		directionString = facingRight ? "_right" : "_left";
		animationPlayer.Play($"hurt{directionString}");
		((Fight)rootNode.GetNode("Fight")).PlayerSetHighscore(-15);

		if (health < 0)
		{
			mainNode.HitOccured(2F);
		}
		else
		{
			mainNode.HitOccured(lightDamageTimer);
		}
	}
}



