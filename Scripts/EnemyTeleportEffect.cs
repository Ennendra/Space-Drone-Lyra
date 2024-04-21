using Godot;
using System;

public partial class EnemyTeleportEffect : Node2D
{

	AnimatedSprite2D sprite;
	AudioStreamPlayer2D audio;

	bool particlesEmitted = false;
	[Export] PackedScene particleEffect;

	bool audioFinished = false, animationFinished = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		audio = GetNode<AudioStreamPlayer2D>("Audio");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (!particlesEmitted && sprite.Frame>=2)
		{
			var emit = particleEffect.Instantiate() as GpuParticles2D;
			GetTree().CurrentScene.AddChild(emit);
			emit.GlobalPosition = GlobalPosition;
			emit.Emitting = true;

			particlesEmitted = true;
		}

		if (audioFinished && animationFinished) 
		{
			QueueFree();
		}
	}

	public void OnAudioFinished()
	{
		audioFinished = true;
	}

	public void OnAnimationFinished()
	{
        Visible = false;
        animationFinished = true;
	}
}
