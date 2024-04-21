using Godot;
using System;

public partial class LevelSelectContainer : Button
{

    [Export] LevelSelectInfo levelInfo;

    public bool isSelected = false;

    TextureRect levelTex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        levelTex = GetNode<TextureRect>("LevelImage");

        if (levelInfo.levelImage != null)
        { levelTex.Texture = levelInfo.levelImage; }
        else
        { levelTex.Texture = null; }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public string GetLevelInfoName()
    {
        return levelInfo.name;
    }

    public string GetLevelInfoDescription()
    {
        return levelInfo.description;
    }

    public PackedScene GetLevelScene()
    {
        return levelInfo.level;
    }

}
