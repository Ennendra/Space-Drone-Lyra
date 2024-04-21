using Godot;
using System;

public partial class DamageIndicator : Label
{

	float secondsToFade = 1.2f;
	Color textColour = new Color(150, 150, 255, 1);
	float currentAlpha = 1.0f;

	float speed;
	Vector2 direction;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		float directionAngle = (float)(-Math.PI + (GD.Randf() * 2 * Math.PI));
		direction = Vector2.FromAngle(directionAngle);

		speed = (float)(GD.Randf()) * 50;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentAlpha -= (1 / secondsToFade) * (float)delta;
		Modulate = new Color(0.7f, 0.7f, 1.0f, currentAlpha);

		GlobalPosition = GlobalPosition + (direction * speed * (float)delta);

		if (currentAlpha <= 0)
		{
			QueueFree();
		}
    }
}
