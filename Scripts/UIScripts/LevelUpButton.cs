using Godot;
using System;

public partial class LevelUpButton : Button
{

	public TextureRect icon;
	public Label lblName, lblDescription, lblLevel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		icon = (TextureRect)FindChild("ButtonIcon");
		lblName = (Label)FindChild("LabelName");
		lblDescription = (Label)FindChild("LabelDescription");
		lblLevel = (Label)FindChild("LabelLevel");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
