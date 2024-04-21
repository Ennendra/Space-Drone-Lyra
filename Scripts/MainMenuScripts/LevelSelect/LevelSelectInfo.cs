using Godot;
using System;

public partial class LevelSelectInfo : Resource
{

    [Export] public string name;
    [Export] public string description;
    [Export] public PackedScene level;
    [Export] public Texture2D levelImage;

    // Called when the node enters the scene tree for the first time.
    public LevelSelectInfo() : this(null, "", "", null) { }

    public LevelSelectInfo(Texture2D newImage, string newName, string newDescription, PackedScene newLevel)
    {
        level = newLevel;
        name = newName;
        description = newDescription;
        levelImage = newImage;

    }
}
