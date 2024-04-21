using Godot;
using System;

public partial class ProjectileGrenade : ProjectileParent
{
    float lifeTimer = 0.0f;
    float maxLifeTime = 0.3f;

    [Export] PackedScene grenadeExplosionScene;
    [Export] PackedScene grenadeClusterScene;
    //Tells us when to spawn cluster projectiles
    [Export] bool isCluster;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        base._Ready();
        double speedvariance = GD.RandRange(-0.3, 0.3);
        speed = speed * (1 + (float)speedvariance);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);

        lifeTimer += (float)delta;

        if (lifeTimer > maxLifeTime) { DetonateProjectile(); }
    }

    public override void DestroyProjectile()
    {

        DetonateProjectile();

        //base.DestroyProjectile();

    }

    void DetonateProjectile()
    {
        var grenadeExplosion = grenadeExplosionScene.Instantiate() as ProjectileParent;
        GetTree().CurrentScene.AddChild(grenadeExplosion);
        
        grenadeExplosion.GlobalPosition = GlobalPosition;
        //newProjectile.Rotation = tempRot; 
        grenadeExplosion.Scale = Scale;
        
        grenadeExplosion.SetDamageStats(hitBoxComponent.damage, 100, hitBoxComponent.damageType);
        

        if (isCluster)
        {
            float projectileAngle = -Mathf.Pi + (2 * Mathf.Pi * GD.Randf());
            int amountOfProjectiles = 20;
            for (int i = 0; i < amountOfProjectiles; i++)
            {
                var clusterRound = grenadeClusterScene.Instantiate() as ProjectileParent;
                GetTree().CurrentScene.AddChild(clusterRound);

                float clusterSpeed = 1400;

                clusterRound.GlobalPosition = GlobalPosition;
                clusterRound.Rotation = projectileAngle; 
                clusterRound.Scale = Scale;
                clusterRound.SetDamageStats(hitBoxComponent.damage * 0.5, 500, hitBoxComponent.damageType);
                clusterRound.speed = clusterSpeed - (clusterSpeed * 0.1f) + (clusterSpeed * 0.2f * GD.Randf());

                projectileAngle += (2 * Mathf.Pi)/amountOfProjectiles;
                if (projectileAngle > (2 * Mathf.Pi))
                {
                    projectileAngle -= (2 * Mathf.Pi);
                }
            }
        }

        QueueFree();
    }
}
