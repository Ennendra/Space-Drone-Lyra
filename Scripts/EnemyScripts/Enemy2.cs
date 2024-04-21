using Godot;
using System;

public partial class Enemy2 : EnemyParent
{

    [Export] float timeToFire = 1.4f;
    float fireTimer;

    [Export] internal PackedScene projectileToSpawn;
    AudioStreamPlayer2D audioWeaponFire;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();

        fireTimer = timeToFire;
        audioWeaponFire = GetNode<AudioStreamPlayer2D>("AudioWeaponFire");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
        fireTimer += (float)delta;
        if (playerTarget != null)
        {
            if (GlobalPosition.DistanceTo(playerTarget.GlobalPosition) <= 400)
            {
                if (fireTimer >= timeToFire)
                {
                    FireWeapon(playerTarget);
                }
            }
        }
    }

    public override Vector2 GetTargetDirection()
    {
        if (playerTarget != null)
        {
            if (GlobalPosition.DistanceTo(playerTarget.GlobalPosition) >= 350)
            {
                return GlobalPosition.DirectionTo(playerTarget.GlobalPosition);
            }
            if (GlobalPosition.DistanceTo(playerTarget.GlobalPosition) <= 120)
            {
                return -GlobalPosition.DirectionTo(playerTarget.GlobalPosition);
            }
        }
        return Vector2.Zero;
    }

    public override void DestroyEnemy()
    {
        levelController.CallDeferred("GenerateNewExplosion1", GlobalPosition);

        base.DestroyEnemy();
    }

    //Fires the weapon at the givenn target
    public virtual void FireWeapon(Player target)
    {
        //reset timers and drop down ammo
        fireTimer = 0 - GD.Randf();
        //get the angle to the target
        float angleToTarget = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();

        //play the weapon sound
        audioWeaponFire.Play();

        //Set the position of the projectile to spawn
        Vector2 spawnPosition = GlobalPosition;
        //create the projectile
        CreateProjectile(spawnPosition, angleToTarget);

    }

    public virtual void CreateProjectile(Vector2 tempPos, float tempRot)
    {
        var newProjectile = projectileToSpawn.Instantiate() as EnemyProjectileParent;
        GetTree().CurrentScene.AddChild(newProjectile);
        newProjectile.GlobalPosition = tempPos;
        newProjectile.Rotation = tempRot;

        newProjectile.SetDamageStats(damage, 550, DamageType.FLAT);

        levelController.AddEProjectileToList(newProjectile);
    }
}
