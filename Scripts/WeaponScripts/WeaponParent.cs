using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

internal enum TargetType
{
    NEAREST,
    HIGHESTHP,
    HIGHESTMAXHP,
    RANDOM
}

public enum DamageType
{
    FLAT,
    PERCENTHP,
    PERCENTMAXHP
}

public partial class WeaponParent : Node2D
{
    //The current level controller to assist with targeting enemies
    public LevelController lControl;
    //The globalscript. Used to access the modifiers e.g. damage
    public GlobalScript globals;

    [Export] public Texture2D weaponIcon;
    [Export] public string weaponName, weaponDescription;

    //the weapon level
    public int weaponLevel = 1;
    [Export] public int maxWeaponLevel = 5;
    public string[] weaponLevelUpDescriptions;

    internal AudioStreamPlayer2D audioWeaponFire;

    //---stats that can potentially be altered by levelling or other factors ---
        //The size of the projectile
        internal float scaleModifier = 1;
        //How many projectiles are created for each time the weapon "fires"
        [Export] internal int projectilesPerShot = 1;
        //How close the enemy needs to be before firing
        [Export] internal protected float distanceToFire = 500.0f;
        //The time it takes to reload -- The rate of fire of the weapon (in shots-per-second)
        [Export] internal double reloadTime = 1, rateOfFire = 10;
        //The max clip of the weapon
        [Export] internal int maxAmmo = 10;
        //the damage of the projectile (either in flat damage or percentage based on the 'isFlatDamage' variable)
        [Export] internal double damage = 3;
        //the weapon scatter in degrees
        [Export] internal float weaponScatter = 3.0f;

    //An enum which determines which enemy to target and a bool which determines whether the projectile is spawned on the weapon, or directly on the target
    [Export] internal TargetType targetType = TargetType.NEAREST;
    [Export] internal bool isDirectHit = false;
    //the current enemy targeted
    public EnemyParent currentTarget;
    //checks whether the damage scaling is flat damage. If false, it does a percentage of HP instead (e.g. a damage of 50 would equal 50% of current HP)
    [Export] internal DamageType damageType = DamageType.FLAT;
    //the projectile instance that needs to be spawned in
    [Export] internal PackedScene projectileToSpawn;
    //A variable which tells us whether the weapon is active
    public bool isActive = true;

    //Variables with get-only processes that are typically affected by global modifiers like damage
    public double GetTimeToFire { get { return 1 / (rateOfFire * globals.gFireRateModifier); } }
    public double GetDamage { get { return damage * globals.gDamageModifier; } }
    public float GetScaleModifier { get { return scaleModifier * globals.gScaleModifier; } }   
    public double GetReloadTime { get { return reloadTime * (1/globals.gReloadSpeedModifier); } }

    //the timer used to determine whether the weapon can fire or not
    public double fireTimer, reloadTimer;
    //The current ammo count
    internal int ammo;
    
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //set the global script
        globals = GetNode<GlobalScript>("/root/Globals");

        //set the weaponfire audio
        audioWeaponFire = GetNode<AudioStreamPlayer2D>("AudioWeaponFire");

        //Set the ammo and timers to be ready to fire
        ammo = maxAmmo;
        fireTimer = GetTimeToFire;
        reloadTimer = reloadTime;
        //set the level controller
        //lControl = GetTree().CurrentScene.GetNodeOrNull<LevelController>("../Main");
        

        //generate the array for weapon level up descriptions (adding +1 to the max so we can leave the '0' array blank and access the others without going '-1' on everything)
        weaponLevelUpDescriptions = new string[maxWeaponLevel + 1];

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //increment timers
        reloadTimer += delta;
        fireTimer += delta;
        //reload weapon if the timer has been reached
        if (reloadTimer >= GetReloadTime)
        {
            ammo = maxAmmo;
        }

        AttemptToFire();
    }
    
    public virtual void LevelUpWeapon()
    {
        //increase the weapon level
        weaponLevel++;
    }

    //a function that checks if we can shoot and have a target to shoot
    public bool CanFire()
    {
        //stop attempting to fire if the weapon is disabled (ie, player dead)
        if (!isActive) return false;
        //check if we havent fired early before and that we have ammo remaining
        if (fireTimer >= GetTimeToFire && ammo > 0)
        {
            //check if the level controller has any active enemies listed
            if (lControl != null)
            {
                if (lControl.enemies.Count > 0) 
                {
                    //check each enemy until we find one that is within range
                    foreach (EnemyParent enemy in lControl.enemies)
                    {
                        //check that the target is within firing distance
                        if (GlobalPosition.DistanceTo(enemy.GlobalPosition) <= distanceToFire)
                        {
                            return true;
                        }
                    }
                }

            }

        }
        
        return false; 
    }

    //Checks whether the weapon can be fired and whether there is a target to fire the weapon at
    public virtual bool AttemptToFire()
    {
        //check if we have ammo and are within rate of fire
        //And also check whether an enemy is within range
        if (!CanFire()) return false;

        currentTarget = null;

        //check if we're targeting the nearest enemy
        currentTarget = ObtainEnemyTarget();

        //did we successfully target an enemy
        if (currentTarget != null)
        {
                FireWeapon(currentTarget);
                return true;
        }
        
        
        //no targets found
        return false;
    }

    public EnemyParent ObtainEnemyTarget()
    {
        //Set initial check values
        EnemyParent targetEnemy = null;
        float distanceToEnemy = -1;
        double targetEnemyHP = -1;
        List<EnemyParent> targetsInRange = new List<EnemyParent>();
        //check each enemy on the field
        foreach (EnemyParent enemy in lControl.enemies)
        {
            //Select the most relevant target based on the enum parameter
            switch (targetType)
            {
                    //target nearest enemy
                case TargetType.NEAREST:
                    {
                        if (IsEnemyCloser(enemy, distanceToEnemy))
                        {
                            targetEnemy = enemy;
                            distanceToEnemy = GlobalPosition.DistanceTo(enemy.GlobalPosition);
                        }
                        break;
                    }
                    //target the enemy with the highest current HP
                case TargetType.HIGHESTHP:
                    {
                        if (IsEnemyHealthHigher(enemy, targetEnemyHP))
                        {
                            targetEnemy = enemy;
                            targetEnemyHP = enemy.GetCurrentHealth();
                        }
                        break;
                    }
                    //target the enemy with the highest maximum HP
                case TargetType.HIGHESTMAXHP:
                    {
                        if (IsEnemyMaxHealthHigher(enemy, targetEnemyHP))
                        {
                            targetEnemy = enemy;
                            targetEnemyHP = enemy.GetMaxHealth();
                        }
                        break;
                    }
                case TargetType.RANDOM:
                    {
                        if (GlobalPosition.DistanceTo(enemy.GlobalPosition) <= distanceToFire)
                        {
                            targetsInRange.Add(enemy);
                        }
                        break;
                    }
            }
        }
        //if we're targeting a random enemy, set one of them as the main target
        if (targetType == TargetType.RANDOM)
        {
            int targetIndex = ((int)GD.Randi()) % (targetsInRange.Count);
            if (targetIndex < 0) {  targetIndex = -targetIndex; }
            targetEnemy = targetsInRange[targetIndex];
        }
        return targetEnemy;
    }

    public bool IsEnemyCloser(EnemyParent enemy, float distanceToCompare)
    {
        float distanceToEnemy = GlobalPosition.DistanceTo(enemy.GlobalPosition);
        if (distanceToEnemy < distanceToCompare || distanceToCompare == -1)
        {
            return true;
        }
        return false;
    }
    public bool IsEnemyHealthHigher(EnemyParent enemy, double healthToCompare)
    {
        if (enemy.GetCurrentHealth() > healthToCompare)
        {
            return true;
        }
        return false;
    }
    public bool IsEnemyMaxHealthHigher(EnemyParent enemy, double healthToCompare)
    {
        if (enemy.GetMaxHealth() > healthToCompare)
        {
            return true;
        }
        return false;
    }

    //Fires the weapon at the givenn target
    public virtual void FireWeapon(EnemyParent target)
    {
        //reset timers and drop down ammo
        fireTimer = 0;
        reloadTimer = 0;
        ammo--;
        //get the angle to the target
        float angleToTarget = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();

        //play the weapon sound
        audioWeaponFire.Play();

        //repeat an amount of times based on the "ProjectilesPerShot" stat
        for (int i = 0; i < projectilesPerShot; i++)
        {
            //Set the position of the projectile to spawn
            Vector2 spawnPosition = GlobalPosition;
            if (isDirectHit)
            {
                //spawn the projectile directly on the target
                spawnPosition = target.GlobalPosition;
            }
            //add scatter to the projectile
            angleToTarget += (-weaponScatter + (GD.Randf()*2*weaponScatter)) * (Mathf.Pi / 180);
            //create the projectile
            CreateProjectile(spawnPosition, angleToTarget);
        }
        
    }

    public virtual ProjectileParent CreateProjectile(Vector2 tempPos, float tempRot)
    {
        var newProjectile = projectileToSpawn.Instantiate() as ProjectileParent;
        GetTree().CurrentScene.AddChild(newProjectile);

        newProjectile.GlobalPosition = tempPos;
        newProjectile.Rotation = tempRot;
        newProjectile.Scale = new Vector2(GetScaleModifier,GetScaleModifier);

        newProjectile.SetDamageStats(damage, distanceToFire + 20.0f, damageType);

        return newProjectile;
    }


}
