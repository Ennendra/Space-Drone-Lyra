using Godot;
using System;

public partial class Enemy7 : EnemyParent
{
    [Export] float timeToFire = 1.4f;
    float fireTimer;

    [Export] internal PackedScene projectileToSpawn;
    AudioStreamPlayer2D audioWeaponFire;

    Vector2 targetPosition = Vector2.Zero;
    double newPositionTimer;
    double newPositionWaitDuration;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        fireTimer = timeToFire;
        audioWeaponFire = GetNode<AudioStreamPlayer2D>("AudioWeaponFire");

        CallDeferred("ChooseNewTargetPosition");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        fireTimer += (float)delta;
        newPositionTimer += (float)delta;
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

        if (newPositionTimer >= newPositionWaitDuration)
        {
            ChooseNewTargetPosition();
        }

    }

    public override Vector2 GetTargetDirection()
    {
        return GlobalPosition.DirectionTo(targetPosition);
    }
    public void ChooseNewTargetPosition()
    {
        if (playerTarget != null)
        {
            targetPosition = playerTarget.GlobalPosition - new Vector2(400, 400) + (GD.Randf() * new Vector2(800, 800));
        }
        newPositionTimer = 0;
        newPositionWaitDuration = GD.RandRange(2.0, 3.5);
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

    public override void DestroyEnemy()
    {
        levelController.CallDeferred("GenerateNewExplosion1", GlobalPosition);

        base.DestroyEnemy();
    }

}
