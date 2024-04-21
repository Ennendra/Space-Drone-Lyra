using Godot;
using System;
using System.Collections.Generic;

public partial class BossEnemy2 : BossParent
{

    

    float currentSpeed = 0;
    Vector2 targetDestination;
    Vector2[] destinations = new Vector2[4];
    float targetAngle;

    bool destinationReached = true;
    float newDestinationTimer = 0;
    int currentDestinationIndex = 0;


    //the fire timers for each weapon
    float[] fireTimer = new float[3];
    [Export] float[] rateOfFire = new float[3];
    float[] timeToFire = new float[3];
    //whether the weapon is active to fire
    bool[] isWeaponActive = new bool[3];
    [Export] double[] weaponDamage = new double[3];
    //a bool for the first weapon so it alternates between 2 fire patterns
    bool weapon1FirstFire = true;
    //variables for weapon 3's tracking before firing
    bool isWeapon3Tracking = false;
    Vector2 weapon3TrackPosition;
    bool hasWeapon3Fired = true;
    float weapon3ChargeTimeLeft = 0;
    //A general reference to a weapon's firing angle (to help simulate rotating weapon fire)
    float generalWeaponAngle = 0;

    //the attached nodes, used to position the projectiles from the weapons
    Node2D weapon1NodeLeft, weapon1NodeRight;
    Node2D weapon2NodeLeft, weapon2NodeRight;
    Node2D weapon3Node;

    //the audio for each weapon
    AudioStreamPlayer2D audioWeaponFire1, audioWeaponFire2, audioWeaponFire3, audioWeaponFire3Lockon;
    //the sprite showing the tracker icon for weapon 3
    Sprite2D trackerSprite;
    Sprite2D connectorSprite, outerRingSprite;
    //The timers to activate and deactivate each weapon
    Timer[] activateTimer = new Timer[3];
    Timer[] deactivateTimer = new Timer[3];

    //an indicator of if the boss is dead and how long it's been dead for (to track when the big boom happens)
    bool isDead = false;
    float deadTimer = 0;
    float deathExplosionTimer, nextDeathExplosionTime;

    //the projectiles spawned from each weapon
    [Export] PackedScene[] weaponProjectiles = new PackedScene[3];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        targetDestination = GlobalPosition;

        //set the fire timer based on the rate of fire
        for (int i = 0; i < timeToFire.Length; i++)
        {
            timeToFire[i] = 1 / rateOfFire[i];
            isWeaponActive[i] = false;
        }
        //set the first weapon to be able to fire
        isWeaponActive[0] = true;

        //get the weapon nodes
        /*
        weapon1NodeLeft = GetNode<Node2D>("Weapon1Left");
        weapon1NodeRight = GetNode<Node2D>("Weapon1Right");
        weapon2NodeLeft = GetNode<Node2D>("Weapon2Left");
        weapon2NodeRight = GetNode<Node2D>("Weapon2Right");*/
        weapon3Node = GetNode<Node2D>("Weapon3");

        //get the audio weapon nodes
        audioWeaponFire1 = GetNode<AudioStreamPlayer2D>("AudioWeaponFire1");
        audioWeaponFire2 = GetNode<AudioStreamPlayer2D>("AudioWeaponFire2");
        audioWeaponFire3 = GetNode<AudioStreamPlayer2D>("AudioWeaponFire3");
        audioWeaponFire3Lockon = GetNode<AudioStreamPlayer2D>("AudioWeaponFire3Lockon");

        //get the tracker sprite
        trackerSprite = GetNode<Sprite2D>("TrackerSprite");
        connectorSprite = GetNode<Sprite2D>("SpriteConnector");
        outerRingSprite = GetNode<Sprite2D>("SpriteOuterRing");

        //get the timer nodes
        activateTimer[0] = GetNode<Timer>("Weapon1Timer_Activate");
        activateTimer[1] = GetNode<Timer>("Weapon2Timer_Activate");
        activateTimer[2] = GetNode<Timer>("Weapon3Timer_Activate");
        deactivateTimer[0] = GetNode<Timer>("Weapon1Timer_Deactivate");
        deactivateTimer[1] = GetNode<Timer>("Weapon2Timer_Deactivate");
        deactivateTimer[2] = GetNode<Timer>("Weapon3Timer_Deactivate");

        SetNewExplosionDeathTimer();
    }

    public override void InitiateSpecialBossBehaviours()
    {
        base.InitiateSpecialBossBehaviours();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!isDead)
        {
            float rotationSpeed = (2 * Mathf.Pi) * (float)delta * 0.2f;
            generalWeaponAngle += rotationSpeed;
            outerRingSprite.Rotation += rotationSpeed;
            connectorSprite.Rotation -= rotationSpeed;


            //process the firerate timers
            for (int i = 0; i < fireTimer.Length; i++)
            {
                fireTimer[i] += (float)delta;
            }

            if (playerTarget != null)
            {
                //fire weapons if within firerate and they are active
                for (int i = 0; i < fireTimer.Length; i++)
                {
                    if (fireTimer[i] >= timeToFire[i] && isWeaponActive[i])
                    {
                        Call("FireWeapon" + (i + 1).ToString(), playerTarget);
                        fireTimer[i] = 0;
                    }
                }

                //prepare the charge and tracking of weapon 3 if it hasn't fired yet
                /*
                if (!hasWeapon3Fired)
                {
                    trackerSprite.Visible = true;
                    trackerSprite.RotationDegrees += 90 * (float)delta;
                    //process the charge timer
                    weapon3ChargeTimeLeft -= (float)delta;
                    //track the player position until a certain time threshold
                    if (isWeapon3Tracking)
                    {
                        weapon3TrackPosition = playerTarget.GlobalPosition;
                        
                        trackerSprite.Modulate = new Color(1, 0, 0, 1);
                        if (weapon3ChargeTimeLeft < 0.75f)
                        {
                            isWeapon3Tracking = false;
                        }
                    }
                    else
                    {
                        trackerSprite.Modulate = new Color(1, 1, 0, 1);
                    }
                    trackerSprite.GlobalPosition = weapon3TrackPosition;
                    if (weapon3ChargeTimeLeft <= 0)
                    {
                        float angleToTracker = GlobalPosition.DirectionTo(weapon3TrackPosition).Angle();
                        CreateProjectile(weaponProjectiles[2], weapon3Node.GlobalPosition, angleToTracker, 2);
                        hasWeapon3Fired = true;
                        audioWeaponFire3.Play();
                    }
                }
                else
                {
                    trackerSprite.Visible = false;
                }*/

            }
        }
        else //Boss is dead
        {
            deathExplosionTimer += (float)delta;
            deadTimer += (float)delta;


            if ( deathExplosionTimer >= nextDeathExplosionTime )
            {
                levelController.CallDeferred("GenerateNewExplosionSmall1", GlobalPosition - new Vector2(50,50) + new Vector2(GD.Randf() * 100, GD.Randf() * 100));
                SetNewExplosionDeathTimer();
            }

            if (deadTimer >= 7)
            {
                FinalDestroy();
            }


        }
    }

    //Weapon Firing
    public void FireWeapon1(Player target)
    {
        int amountOfProjectiles = 16;
        float projectileAngle = generalWeaponAngle/2;

        if (!weapon1FirstFire)
            projectileAngle += ((2 * Mathf.Pi) / amountOfProjectiles)/2;


        for (int i = 0; i < amountOfProjectiles; i++)
        {
            Vector2 projectileOffset = new Vector2(54, 0).Rotated(projectileAngle);

            CreateProjectile(weaponProjectiles[0], GlobalPosition+projectileOffset, projectileAngle, 0);

            projectileAngle += (2 * Mathf.Pi) / amountOfProjectiles;
            if (projectileAngle > (2 * Mathf.Pi))
            {
                projectileAngle -= (2 * Mathf.Pi);
            }
        }
        

        weapon1FirstFire = !weapon1FirstFire;
        audioWeaponFire1.Play();
    }
    public void FireWeapon2(Player target)
    {
        int amountOfProjectiles = 6;
        float projectileAngle = generalWeaponAngle;

        for (int i = 0; i < amountOfProjectiles; i++)
        {
            Vector2 projectileOffset = new Vector2(54, 0).Rotated(projectileAngle);

            CreateProjectile(weaponProjectiles[1], GlobalPosition + projectileOffset, projectileAngle, 1);

            projectileAngle += (2 * Mathf.Pi) / amountOfProjectiles;
            if (projectileAngle > (2 * Mathf.Pi))
            {
                projectileAngle -= (2 * Mathf.Pi);
            }
        }

        audioWeaponFire2.Play();
    }
    public void FireWeapon3Old(Player target)
    {
        isWeapon3Tracking = true;
        hasWeapon3Fired = false;
        weapon3ChargeTimeLeft = 2.5f;
    }

    public async void FireWeapon3(Player target)
    {
        float projectileAngle1, projectileAngle2;
        Vector2 projectileOffset;

        List<EnemyProjectileParent> lockonProjectiles = new List<EnemyProjectileParent>();

        //Fire initial set of projectiles in sequence
        for (int i = 0; i<3; i++)
        {


            projectileAngle1 = generalWeaponAngle + (Mathf.Pi / 2);
            projectileAngle2 = generalWeaponAngle - (Mathf.Pi / 2);

            projectileAngle1 += Mathf.Pi / 1 / (i + 1);
            projectileAngle2 -= Mathf.Pi / 1 / (i + 1);

            projectileOffset = new Vector2(54, 0).Rotated(projectileAngle1);
            lockonProjectiles.Add(CreateProjectile(weaponProjectiles[2], GlobalPosition + projectileOffset, projectileAngle1, 2));
            projectileOffset = new Vector2(54, 0).Rotated(projectileAngle2);
            lockonProjectiles.Add(CreateProjectile(weaponProjectiles[2], GlobalPosition + projectileOffset, projectileAngle2, 2));

            audioWeaponFire3.Play();

            SceneTreeTimer fireDelayTimer = GetTree().CreateTimer(0.25f, false);
            await ToSignal(fireDelayTimer, SceneTreeTimer.SignalName.Timeout);
        }

        SceneTreeTimer lockOnTimer = GetTree().CreateTimer(1.2f, false);
        await ToSignal(lockOnTimer, SceneTreeTimer.SignalName.Timeout);

        //lockon to player
        foreach(EnemyProjectileParent projectile in lockonProjectiles)
        {
            projectile.EmitSignal("LockOnTargetSignal", target);
        }
        audioWeaponFire3Lockon.Play();

    }

    public EnemyProjectileParent CreateProjectile(PackedScene projectileToSpawn, Vector2 newPosition, float newRotation, int weaponIndex)
    {
        var newProjectile = projectileToSpawn.Instantiate() as EnemyProjectileParent;
        GetTree().CurrentScene.AddChild(newProjectile);

        newProjectile.GlobalPosition = newPosition;
        newProjectile.Rotation = newRotation;

        newProjectile.SetDamageStats(weaponDamage[weaponIndex], 1200.0f, DamageType.FLAT);

        if (weaponIndex == 1)
        {
            newProjectile.Modulate = new Color(1,0,1,1);
        }

        return newProjectile;
    }

    

    public override void DestroyEnemy()
    {
        isDead = true;
    }

    public void SetNewExplosionDeathTimer()
    {
        deathExplosionTimer = 0;
        nextDeathExplosionTime = 0.3f + (GD.Randf() * 0.5f);
    }

    public void FinalDestroy()
    {
        levelController.CallDeferred("GenerateNewExplosion2", GlobalPosition);

        wall.QueueFree();

        base.DestroyEnemy();
    }

    //The timeout signals for the weapon timers
    public void TimerWeapon1Activate()
    {
        isWeaponActive[0]= true;
        deactivateTimer[0].Start();
    }
    public void TimerWeapon1Deactivate()
    {
        isWeaponActive[0] = false;
        activateTimer[0].Start();
    }
    public void TimerWeapon2Activate()
    {
        isWeaponActive[1] = true;
        deactivateTimer[1].Start();
    }
    public void TimerWeapon2Deactivate()
    {
        isWeaponActive[1] = false;
        activateTimer[1].Start();
    }
    public void TimerWeapon3Activate()
    {
        isWeaponActive[2] = true;
        deactivateTimer[2].Start();
    }
    public void TimerWeapon3Deactivate()
    {
        isWeaponActive[2] = false;
        activateTimer[2].Start();
    }
}
