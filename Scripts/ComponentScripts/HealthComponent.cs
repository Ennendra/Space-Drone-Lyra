using Godot;
using GodotPlugins.Game;
using System;

public partial class HealthComponent : Node2D
{
    Node2D parentNode;

    [Export] double baseMaxHealth = 100;

    [Export] AnimatedSprite2D spriteToFlash;
    public double maxHealth { get; private set; }
    public double health { get; private set; }
    double healthRegen = 0;

    //a check on whether this character is "dead"
    internal bool isDead = false;
    //used for showing "flash" damage animations in the parent object
    internal double flashTimer = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        parentNode = GetParent<Node2D>();

        //set the health to the base max HP
        maxHealth = baseMaxHealth;
        health = baseMaxHealth;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //process the flash timer
        flashTimer += delta;
        if (flashTimer > 0.06)
        {
            if (spriteToFlash != null) spriteToFlash.Visible = false;
        }

        //process health regen
        double initialHealth = health;
        health += healthRegen * delta;
        if (health > maxHealth) { health = maxHealth; }
    }

    //Increasing maximum HP and adding that to current HP
    
	public void ChangeMaxHealth(double percentChange)
	{
		double healthChangeAmount = baseMaxHealth * (1+percentChange);
		maxHealth += healthChangeAmount;
		health += healthChangeAmount;
        
	}

    public void ChangeHealthRegen(double amount)
    {
        healthRegen += amount;
    }

    public void TakeDamage(double damageTaken)
    {
        if (!isDead)
        {
            health -= damageTaken;

            flashTimer = 0;
            if (spriteToFlash != null) spriteToFlash.Visible = true;

            if (health < 0 && !isDead)
            {
                parentNode.EmitSignal("deathCall");

                isDead = true;
            }
            else
            {
                //audioTakeHit.Play();
            }
        }

    }
}
