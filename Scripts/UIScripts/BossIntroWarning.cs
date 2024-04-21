using Godot;
using System;

public partial class BossIntroWarning : CanvasLayer
{

	Control cBar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cBar = GetNode<Control>("CBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		cBar.Position = cBar.Position - new Vector2(-(150*(float)delta), 0);
	}
}
