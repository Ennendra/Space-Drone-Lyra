using Godot;
using System;

public partial class PlayerInfo : Resource
{
	[Export] public string name;
    [Export] public string description;
    [Export] public PackedScene playerToInstantiate;
	[Export] public Texture2D playerImage, starterWeapon, starterItem;

	public PlayerInfo() : this(null, "","",null, null, null) { }

	public PlayerInfo(Texture2D newImage, string newName, string newDescription, PackedScene newPlayer, Texture2D newWeapon, Texture2D newItem)
	{
		playerImage = newImage;
		name = newName;
		description = newDescription;
		playerToInstantiate = newPlayer;
		starterWeapon = newWeapon;
		starterItem = newItem;

	}
}
