using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyProjectileParent : Area2D
{

    //Components
    [Export] HitBoxComponent hitBoxComponent;

    public float distanceToDeath = 500.0f; //how far in pixels the projectile can fly before detonating
    [Export] ProjectileMovementBehaviour movementBehaviour = ProjectileMovementBehaviour.NORMAL;
    //projectile speed in pixels per second
    [Export] internal float speed = 800;

	float distanceCovered = 0.0f;
	

    PackedScene deathParticle = (PackedScene)ResourceLoader.Load("res://Objects/FX/ProjectileParticles.tscn");
    PackedScene deathADSParticle = (PackedScene)ResourceLoader.Load("res://Objects/FX/ProjectileParticleADS.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		

        AddUserSignal("emitParticle");
        Connect("emitParticle", new Callable(this, "CreateDeathParticle"));
        AddUserSignal("emitADSParticle");
        Connect("emitADSParticle", new Callable(this, "CreateDeathParticleADS"));

        AddUserSignal("deathSignal");
        Connect("deathSignal", new Callable(this, "DestroyProjectile"));

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		if (movementBehaviour == ProjectileMovementBehaviour.NORMAL)
		{
			//cast a ray to see if it hits anything in its path
			var spaceState = GetWorld2D().DirectSpaceState;

			var cast = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + (Transform.X * speed * (float)delta), this.CollisionMask);
			var result = spaceState.IntersectRay(cast);

			if (result.Count > 0)
			{
				GlobalPosition = (Vector2)result["position"];
			}
			else
			{
                Vector2 newMovement = Transform.X * speed * (float)delta;
                //move the projectile forward based on speed
                GlobalPosition += newMovement;

				//process the distance and destroy the projectile if it's reached its limit
				distanceCovered += newMovement.Length();
				if (distanceCovered >= distanceToDeath)
				{
					DestroyProjectile();
				}
			}
		}
        else if (movementBehaviour == ProjectileMovementBehaviour.SIMPLEMOVEMENT)
        {
            //move the projectile forward based on speed
            GlobalPosition += Transform.X * speed * (float)delta;

            //process the distance and destroy the projectile if it's reached its limit
            distanceCovered += (Transform.X * speed * (float)delta).Length();
            if (distanceCovered >= distanceToDeath)
            {
                DestroyProjectile();
            }
        }
    }

	public void SetDamageStats(double damage, float newDistanceToDeath, DamageType damageType)
	{
		hitBoxComponent.damage = damage;
		hitBoxComponent.damageType = damageType;

		distanceToDeath = newDistanceToDeath;
	}

    public void CreateDeathParticle()
    {
        Vector2 particlePosition = this.GlobalPosition;

        var newParticle = deathParticle.Instantiate() as GpuParticles2D;
        GetTree().CurrentScene.AddChild(newParticle);

        newParticle.GlobalPosition = particlePosition;
        newParticle.Emitting = true;
    }

    public void CreateDeathParticleADS()
    {
        Vector2 particlePosition = this.GlobalPosition;

        var newParticle = deathADSParticle.Instantiate() as GpuParticles2D;
        GetTree().CurrentScene.AddChild(newParticle);

        newParticle.GlobalPosition = particlePosition;
        newParticle.GlobalRotation = GlobalRotation;
        newParticle.Emitting = true;
    }

    //destroying the projectile
    public void DestroyProjectile()
	{
        QueueFree();
    }
}
