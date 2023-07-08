using Godot;
using System;

public partial class DamageTimerEnemy : Timer
{
	Enemy parent;
	AnimationPlayer animationPlayer;
	bool timerJustStarted;
	bool timerCounting;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = (Enemy)this.GetParent();
		timerJustStarted = false;
		timerCounting = false;
		animationPlayer = parent.GetNode<AnimationPlayer>("AnimationPlayer");
		this.Timeout += () => {
			timerCounting = false;
			parent.damagePaused = false;
			animationPlayer.Play();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!timerCounting && !timerJustStarted && !this.IsStopped())
		{
			timerJustStarted = true;
			timerCounting = true;
		}

		if (timerJustStarted)
		{
			timerJustStarted = false;
			parent.damagePaused = true;
			animationPlayer.Pause();
		}
	}
}
