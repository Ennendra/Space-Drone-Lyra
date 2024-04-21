using Godot;
using System;

public partial class BossEnemy1 : BossParent
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

    //the attached nodes, used to position the projectiles from the weapons
    Node2D weapon1NodeLeft, weapon1NodeRight;
    Node2D weapon2NodeLeft, weapon2NodeRight;
    Node2D weapon3Node;
    //the audio for each weapon
    AudioStreamPlayer2D audioWeaponFire1, audioWeaponFire2, audioWeaponFire3;
    //the sprite showing the tracker icon for weapon 3
    Sprite2D trackerSprite;
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
        weapon1NodeLeft = GetNode<Node2D>("Weapon1Left");
        weapon1NodeRight = GetNode<Node2D>("Weapon1Right");
        weapon2NodeLeft = GetNode<Node2D>("Weapon2Left");
        weapon2NodeRight = GetNode<Node2D>("Weapon2Right");
        weapon3Node = GetNode<Node2D>("Weapon3");

        //get the audio weapon nodes
        audioWeaponFire1 = GetNode<AudioStreamPlayer2D>("AudioWeaponFire1");
        audioWeaponFire2 = GetNode<AudioStreamPlayer2D>("AudioWeaponFire2");
        audioWeaponFire3 = GetNode<AudioStreamPlayer2D>("AudioWeaponFire3");

        //get the tracker sprite
        trackerSprite = GetNode<Sprite2D>("TrackerSprite");

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
        GenerateMovementLocations();
    }

    //Generate the positions the boss can move to during the fight
    public void GenerateMovementLocations()
    {
        destinations[0] = new Vector2(arenaCenter.X, arenaCenter.Y - arenaSize.Y + 80);
        destinations[1] = new Vector2(arenaCenter.X + arenaSize.X - 80, arenaCenter.Y);
        destinations[2] = new Vector2(arenaCenter.X, arenaCenter.Y + arenaSize.Y - 80);
        destinations[3] = new Vector2(arenaCenter.X - arenaSize.X + 80, arenaCenter.Y);
    }

    //choose a new location based on the current one from the list generated above
    public void ChooseNewLocation()
    {
        //choose to go up or down the desination index based on an RNG check
        float RNGCheck = GD.Randf();
        int indexDirection = currentDestinationIndex;
        if (RNGCheck < 0.5f)
        {
            indexDirection += 1;
        }
        else
        {
            indexDirection -=1;
        }

        //shift the index if it goes above or below the bounds
        if (indexDirection == -1) { indexDirection = 3; }
        if (indexDirection == 4) { indexDirection = 0; }

        //set the target destination and reset the movement variables
        currentDestinationIndex = indexDirection;
        targetDestination = destinations[currentDestinationIndex];
        destinationReached = false;
        newDestinationTimer = 0;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!isDead)
        {
            //process the firetimers
            for (int i = 0; i < fireTimer.Length; i++)
                { fireTimer[i] += (float)delta; }

            if (playerTarget != null)
            {
                for (int i = 0; i < fireTimer.Length; i++)
                {
                    if (fireTimer[i] >= timeToFire[i] && isWeaponActive[i])
                    {
                        Call("FireWeapon" + (i + 1).ToString(), playerTarget);
                        fireTimer[i] = 0;
                    }
                }

                //prepare the charge and tracking of weapon 3 if it hasn't fired yet
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
                }

            }
        }
        else //Boss is dead
        {
            RotationDegrees += 45 * (float)delta;
            deadTimer += (float)delta;

            deathExplosionTimer += (float)delta;
            if ( deathExplosionTimer >= nextDeathExplosionTime )
            {
                levelController.CallDeferred("GenerateNewExplosionSmall1", GlobalPosition - new Vector2(50,50) + new Vector2(GD.Randf() * 100, GD.Randf() * 100));
                SetNewExplosionDeathTimer();
            }


            if (deadTimer >= 5)
            {
                FinalDestroy();
            }


        }
    }

    //weapon 1 - the rapid firing weapon with 2 alternating fire patterns
    public void FireWeapon1(Player target)
    {
        //Mathf.DegToRad(15);
        if (weapon1FirstFire)
        {
            CreateProjectile(weaponProjectiles[0], weapon1NodeLeft.GlobalPosition,Rotation, 0);
            CreateProjectile(weaponProjectiles[0], weapon1NodeRight.GlobalPosition, Rotation, 0);

            CreateProjectile(weaponProjectiles[0], weapon1NodeLeft.GlobalPosition, Rotation + Mathf.DegToRad(35), 0);
            CreateProjectile(weaponProjectiles[0], weapon1NodeRight.GlobalPosition, Rotation + Mathf.DegToRad(35), 0);

            CreateProjectile(weaponProjectiles[0], weapon1NodeLeft.GlobalPosition, Rotation - Mathf.DegToRad(35), 0);
            CreateProjectile(weaponProjectiles[0], weapon1NodeRight.GlobalPosition, Rotation - Mathf.DegToRad(35), 0);
        }
        else
        {
            CreateProjectile(weaponProjectiles[0], weapon1NodeLeft.GlobalPosition, Rotation + Mathf.DegToRad(15), 0);
            CreateProjectile(weaponProjectiles[0], weapon1NodeRight.GlobalPosition, Rotation + Mathf.DegToRad(15), 0);

            CreateProjectile(weaponProjectiles[0], weapon1NodeLeft.GlobalPosition, Rotation - Mathf.DegToRad(15), 0);
            CreateProjectile(weaponProjectiles[0], weapon1NodeRight.GlobalPosition, Rotation - Mathf.DegToRad(15), 0);
        }
        weapon1FirstFire = !weapon1FirstFire;
        audioWeaponFire1.Play();
    }
    public void FireWeapon2(Player target)
    {
        float degreeVariance = -40;
        for (int i = 0; i < 5; i++)
        {
            CreateProjectile(weaponProjectiles[1], weapon2NodeLeft.GlobalPosition, Rotation + Mathf.DegToRad(degreeVariance), 0);
            CreateProjectile(weaponProjectiles[1], weapon2NodeRight.GlobalPosition, Rotation + Mathf.DegToRad(degreeVariance), 0);
            degreeVariance += 20;
        }
        audioWeaponFire2.Play();
    }
    public void FireWeapon3(Player target)
    {
        isWeapon3Tracking = true;
        hasWeapon3Fired = false;
        weapon3ChargeTimeLeft = 2.5f;
    }

    public void CreateProjectile(PackedScene projectileToSpawn, Vector2 newPosition, float newRotation, int weaponIndex)
    {
        var newProjectile = projectileToSpawn.Instantiate() as EnemyProjectileParent;
        GetTree().CurrentScene.AddChild(newProjectile);

        newProjectile.GlobalPosition = newPosition;
        newProjectile.Rotation = newRotation;

        newProjectile.SetDamageStats(weaponDamage[weaponIndex], 2500.0f, DamageType.FLAT);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isDead)
        {
            //rotate towards the playercharacter
            targetAngle = GetTargetDirection().Angle();
            Rotation = Mathf.LerpAngle(Rotation, targetAngle, 3.5f * (float)delta);
            //move towards one of the desinations if not already reached
            if (!destinationReached)
            {
                Accelerate((float)delta * speed);
                if (GlobalPosition.DistanceTo(targetDestination) < 150)
                {
                    destinationReached = true;
                }
            }
            else
            {
                //slow down and prepare to choose a new spot
                Decelerate((float)delta * speed);
                newDestinationTimer += (float)delta;
                if (newDestinationTimer > 2)
                {
                    ChooseNewLocation();
                }
            }
        }
        else
        {
            Decelerate((float)delta * (speed/2));
        }

        Vector2 velocity = GlobalPosition.DirectionTo(targetDestination) * currentSpeed * (float)delta;

        GlobalPosition += velocity;
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

    //functions to increase and decrease speed
    public void Accelerate(float amount)
    {
        currentSpeed += amount;
        if (currentSpeed > speed) { currentSpeed = speed; }
    }
    public void Decelerate(float amount)
    {
        currentSpeed -= amount;
        if (currentSpeed < 0 ) { currentSpeed = 0; }
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
