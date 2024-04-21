using Godot;
using System;

public partial class ProjectileBoss1_3 : EnemyProjectileParent
{
    float lifeTimer = 0.0f;
    float maxLifeTime = 0.3f;
    Sprite2D sprite;

    public override void _Ready()
    {
        base._Ready();

        sprite = GetNode<Sprite2D>("Sprite");
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        lifeTimer += (float)delta;
        float newAlpha = 1 - (lifeTimer / maxLifeTime);
        sprite.Modulate = new Color(1.0f, 1.0f, 1.0f, newAlpha);


        if (lifeTimer > maxLifeTime) { QueueFree(); }
    }
}
