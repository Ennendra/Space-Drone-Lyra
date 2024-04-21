using Godot;
using System;

public partial class MainMenuCharacterContainer : Button
{

	[Export] PlayerInfo playerInfo;

	public bool isSelected = false;

	TextureRect playerTex, weaponTex, itemTex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerTex = GetNode<TextureRect>("PlayerImage");
        weaponTex = GetNode<TextureRect>("StarterWeapon");
        itemTex = GetNode<TextureRect>("StarterItem");

		if (playerInfo.playerImage!=null)
		{ playerTex.Texture = playerInfo.playerImage; }
		else
		{ playerTex.Texture = null; }

        if (playerInfo.starterWeapon != null)
        { weaponTex.Texture = playerInfo.starterWeapon; }
        else
        { weaponTex.Texture = null; }

        if (playerInfo.starterItem != null)
        { itemTex.Texture = playerInfo.starterItem; }
        else
        { itemTex.Texture = null; }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public string GetPlayerInfoName()
	{
		return playerInfo.name;
	}

	public string GetPlayerInfoDescription()
	{
		return playerInfo.description;
	}

	public PackedScene GetPlayerScene()
	{
		return playerInfo.playerToInstantiate;
	}
}
