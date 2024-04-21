using Godot;
using System;

public partial class Explosion1 : AnimatedSprite2D
{

	bool animationEnded = false, audioEnded = false;

    PackedScene particle = (PackedScene)ResourceLoader.Load("res://Objects/FX/Explosion1Particles.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        CallDeferred("CreateParticle");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (animationEnded && audioEnded)
		{
            QueueFree();
        }
	}

    public void CreateParticle()
    {
        var newParticle = particle.Instantiate() as GpuParticles2D;
        GetTree().CurrentScene.AddChild(newParticle);

        newParticle.GlobalPosition = this.GlobalPosition;
        newParticle.Emitting = true;
    }

    public void OnAnimationFinished()
	{
        Visible = false;
        animationEnded = true;
    }

	public void OnAudioEnd()
	{
		audioEnded = true;
	}

}
