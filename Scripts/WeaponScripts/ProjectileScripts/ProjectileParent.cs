using Godot;
using System;
using System.Collections.Generic;


//enum which dictates how the object will move in the physicsprocess
//NORMAL - movement done via raycast, will stop when hitting an enemy or move the max movespeed
//SIMPLEMOVEMENT - will simply move based on the movespeed. Mostly used for piercing rounds etc.
//NOMOVEMENT - will not move at all. Used for instant-shot weapons (like the upgraded sniper round)
public enum ProjectileMovementBehaviour
{
    NORMAL,
    SIMPLEMOVEMENT,
    NOMOVEMENT
}

public partial class ProjectileParent : Area2D
{
    //components
    [Export] internal HitBoxComponent hitBoxComponent;

	//stats that can be altered by the weapon
    public float distanceToDeath = 500.0f; //how far in pixels the projectile can fly before detonating
    //projectile speed in pixels per second
    [Export] internal float speed = 800;

    //Determines how the projectile will operate when it hits enemies, see the enum above
    [Export] ProjectileMovementBehaviour movementBehaviour = ProjectileMovementBehaviour.NORMAL;

    

	float distanceCovered = 0.0f;
	protected GlobalScript globals;

	PackedScene deathParticle = (PackedScene)ResourceLoader.Load("res://Objects/FX/ProjectileParticles.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		globals = GetNode<GlobalScript>("/root/Globals");

        AddUserSignal("emitParticle");
        AddUserSignal("deathSignal");
        Connect("emitParticle", new Callable(this, "CreateDeathParticle"));
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
            var cast = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + (Transform.X * speed * (float)delta),hitBoxComponent.CollisionMask);
            cast.CollideWithAreas = true; //set collide with areas to true, so it will register hitting a "hurtbox"
            var collisionResult = spaceState.IntersectRay(cast);

            if (collisionResult.Count > 0)
            {
                //set the position at the point of contact
                GlobalPosition = (Vector2)collisionResult["position"];
            }
            else
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

    public virtual void SetDamageStats(double damage, float newDistanceToDeath, DamageType damageType)
    {
        hitBoxComponent.damage = damage * globals.gDamageModifier;
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

	//destroying the projectile
	public virtual void DestroyProjectile()
	{
        QueueFree();
    }
}
