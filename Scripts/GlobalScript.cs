using Godot;
using System;
using System.Collections.Generic;



public partial class GlobalScript : Node
{
	public Node CurrentScene { get; set; }

	//modifiers that can be affected by items etc
	public float gDamageModifier = 1;
	public float gScaleModifier = 1;
	public float gFireRateModifier = 1;
	public float gReloadSpeedModifier = 1;
	public float gExperienceModifier = 1;
	public float gExperienceMagnetScaleModifier = 1;
	/*NOTE: The following modifiers are *not* affected by the globalscript:
	 - Health regen (This will be changed directly on the player)
	 - Max HP (This will also be directly changed on the player)
	 */



	public Player playerCharacter;

	public int playerMaxWeapons = 3;
    public int playerMaxItems = 4;

    PackedScene indicatorScene = (PackedScene)ResourceLoader.Load("res://Objects/DamageIndicator.tscn");

	public PackedScene playerToCreate;

	//the mission timers, usually operated by the levelcontroller and seen by the mainUI
    public double tMinutes = 0, tSeconds = 0, tSecFraction = 0;

	public int enemiesKilled = 0;

    public override void _Ready()
	{
		//set the current scene
		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);

		playerToCreate = (PackedScene)ResourceLoader.Load("res://Objects/Players/Player.tscn");

		//randomise the GD randomiser
		GD.Randomize();
    }

	public void SetNewStarterPlayer(string path)
	{
		playerToCreate = (PackedScene)ResourceLoader.Load(path);
	}

	public void SetPlayer(Player newPlayer)
	{
		playerCharacter = newPlayer;
		playerCharacter.SetMaxEquipmentValues(playerMaxWeapons, playerMaxItems);
	}

	public void ResetLevel()
	{
		ResetGlobalModifiers();
		ResetLevelTimer();

		enemiesKilled = 0;

        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

	public void ResetGlobalModifiers()
	{

		//reset all the scaling modifiers
        gDamageModifier = 1;
		gScaleModifier = 1;
		gFireRateModifier = 1;
		gReloadSpeedModifier = 1;
        gExperienceModifier = 1;
		gExperienceMagnetScaleModifier = 1;
        
	}

	public void ResetLevelTimer()
	{
		tSecFraction = 0;
		tSeconds = 0;
		tMinutes = 0;
	}

	public void GenerateDamageIndicator(double damage, Vector2 position)
	{
		Label newIndicator = (Label)indicatorScene.Instantiate();
		CurrentScene.AddChild(newIndicator);
		int roundedDamage = ((int)Math.Floor(damage));
		newIndicator.Text = roundedDamage.ToString();
		newIndicator.GlobalPosition = position;
	}

	

	//The following are scripts called by items upon levelup to change certain scalings. Set as functions for easier readability
	public void ChangeDamageModifier(float changeAmount)
	{
		gDamageModifier += changeAmount;
	}
    public void ChangeScaleModifier(float changeAmount)
    {
        gScaleModifier += changeAmount;
    }
    public void ChangeFireRateModifier(float changeAmount)
    {
        gFireRateModifier += changeAmount;
    }
    public void ChangeReloadSpeedModifier(float changeAmount)
    {
        gReloadSpeedModifier += changeAmount;
    }
    public void ChangeExperienceModifier(float changeAmount)
    {
        gExperienceModifier += changeAmount;
    }
    public void ChangeExperienceMagnetScaleModifier(float changeAmount)
    {
        gExperienceMagnetScaleModifier += changeAmount;
		playerCharacter.ChangeMagnetScale(gExperienceMagnetScaleModifier);
    }
	public void ChangeMaxHPAmount(double healthPercentAmount)
	{
		playerCharacter.ChangeMaxHealth(healthPercentAmount);
	}
	public void ChangeHPRegenAmount(double changeAmount)
	{
		playerCharacter.ChangeHealthRegen(changeAmount);
	}
}
