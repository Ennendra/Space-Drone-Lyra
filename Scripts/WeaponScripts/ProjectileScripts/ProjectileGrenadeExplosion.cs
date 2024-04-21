using Godot;
using System;

public partial class ProjectileGrenadeExplosion : ProjectileParent
{

    bool animationEnded = false, audioEnded = false;

    float lifeTimer = 0.0f;
    float maxLifeTime = 0.15f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        base._Ready();
    
    }



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);

        lifeTimer += (float)delta;
        if (lifeTimer > maxLifeTime)
        {
            hitBoxComponent.Monitoring = false;
            hitBoxComponent.Monitorable = false;
        }


        if (animationEnded && audioEnded)
        {
            QueueFree();
        }
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
