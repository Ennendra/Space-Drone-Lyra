using Godot;
using System;

public partial class ScrollingBackground : ParallaxBackground
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ScrollOffset -= new Vector2(80,0) * (float)delta;
		if (ScrollOffset.X < 1024)
		{
			ScrollOffset += new Vector2(1024, 0);
		}
	}
}
