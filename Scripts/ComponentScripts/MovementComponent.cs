using Godot;
using System;

public partial class MovementComponent : Area2D
{

	Vector2 knockBack = Vector2.Zero;
	Vector2 direction = Vector2.Zero;
	float knockBackFriction = 1;
	public float knockBackAmount = 2.25f;
	[Export] float speed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		knockBack = knockBack.MoveToward(Vector2.Zero, knockBackFriction);

		SetMovementVelocity(delta);
    }

	public void SetNewDirection(Vector2 newDirection)
	{
		direction = newDirection;
	}

    public Vector2 SetMovementVelocity(double delta)
	{
		Node2D parent = (Node2D)this.GetParent();

		Vector2 velocity = direction * speed * (float)delta;
		velocity += knockBack;
		parent.GlobalPosition += velocity;

		return velocity;
	}

	public void SetKnockback(Vector2 amount)
	{
		knockBack = amount;
	}

	public void OnAreaEntered(Area2D area)
	{
		var overlappingAreas = GetOverlappingAreas();

		foreach (Area2D a in overlappingAreas)
		{
			if (a is MovementComponent)
			{
				MovementComponent otherComponent = (MovementComponent)a;
				otherComponent.SetKnockback(GlobalPosition.DirectionTo(otherComponent.GlobalPosition) * knockBackAmount);
			}
		}
	}
}
