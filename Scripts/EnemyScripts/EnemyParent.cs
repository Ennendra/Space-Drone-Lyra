using Godot;
using System;

public partial class EnemyParent : Area2D
{

	[Export] internal float speed=80;

	//tells us if the enemy is queued to die. This helps prevent multiple explosions and experience pips being spawned from say, a shotgun blast
	bool isDead = false;

	public Player playerTarget;
	public LevelController levelController;

    //Components
    [Export] HurtBoxComponent hurtBoxComponent;
    [Export] HitBoxComponent hitBoxComponent;
	[Export] HealthComponent healthComponent;
	[Export] MovementComponent movementComponent;

    //whether this enemy deals damage on contact
    [Export] public bool dealsDamageOnContact = true;
	//the damage the enemy deals (in damage-per-second)
	[Export] internal double damage = 10;
	public double damagetimer=1, timeToDamage = 0.2;

    AnimatedSprite2D spriteMain, spriteFlash;
	//how much XP this enemy gives to the player on death
	[Export] float experience = 3;

	AudioStreamPlayer2D audioTakeHit;
	int framesAlive = 0;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		//connect signals
		AddUserSignal("deathCall");
        Connect("deathCall", new Callable(this, "DestroyEnemy"));

        //connect signals
        AddUserSignal("leaveCall");
        Connect("leaveCall", new Callable(this, "TeleportEnemyAway"));

        spriteMain = GetNode<AnimatedSprite2D>("Sprite"); 
        spriteFlash = GetNode<AnimatedSprite2D>("SpriteDamage");
        audioTakeHit = GetNode<AudioStreamPlayer2D>("AudioTakeHit");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		damagetimer += delta;
	}

    public override void _PhysicsProcess(double delta)
    {
		
        base._PhysicsProcess(delta);

		Vector2 movement = GetTargetDirection();

		if (movementComponent != null)
		{
			movementComponent.SetNewDirection(movement);
			//GlobalPosition += movementComponent.SetMovementVelocity(delta);
		}

		if (movement.X < -0.1)
		{
            spriteMain.FlipH = true;
            spriteFlash.FlipH = true;
        }
		if (movement.X > 0.1)
		{
            spriteMain.FlipH = false;
            spriteFlash.FlipH = false;
        }

		
    }

    //move towards the target position
    public virtual Vector2 GetTargetDirection() 
	{
		if (playerTarget != null)
		{
			return GlobalPosition.DirectionTo(playerTarget.GlobalPosition);
		}
		return Vector2.Zero;
	}

	public virtual void DestroyEnemy()
	{
        isDead = true;
        //remove this enemy from the level controller list
        levelController.RemoveEnemy(this,true);
		//add an XP pip to the field at the place of death (using calldeferred to avoid clashes with collision processing)
		if (experience > 0)
			levelController.CallDeferred("CreateExperiencePip", GlobalPosition, experience);
		//destroy the object
        QueueFree();
    }

	public async void TeleportEnemyAway()
	{
		double timeToJump = GD.RandRange(1.5, 4.0);
        SceneTreeTimer introTimer = GetTree().CreateTimer(timeToJump, false);
        await ToSignal(introTimer, SceneTreeTimer.SignalName.Timeout);

		//TODO add instantiate for a teleport effect
		levelController.GenerateTeleportEffect(GlobalPosition);

		//create a small timer so the enemy disappears as the animation hits a higher point.
        SceneTreeTimer outroTimer = GetTree().CreateTimer(2/18, false);
        await ToSignal(outroTimer, SceneTreeTimer.SignalName.Timeout);

        levelController.RemoveEnemy(this, false);
        QueueFree();

    }

	public double GetCurrentHealth()
	{
		return healthComponent.health;
	}
    public double GetMaxHealth()
    {
        return healthComponent.maxHealth;
    }
}
