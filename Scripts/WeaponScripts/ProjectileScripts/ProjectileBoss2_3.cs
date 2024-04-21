using Godot;
using System;

public partial class ProjectileBoss2_3 : EnemyProjectileParent
{
    [Signal] public delegate void LockOnTargetSignalEventHandler(Player target);

    float targetAngle;
    bool isLockedOn = false;

    public override void _Ready()
    {
        base._Ready();

        //setting the lockon signal to the function
        LockOnTargetSignal += LockingOnTarget;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!isLockedOn)
        {
            Decelerate((float)delta * 400);
        }
        else
        {
            Accelerate((float)delta * 1000);
        }
    }

    public void Accelerate(float amount)
    {
        speed += amount;
        if (speed > 1200) { speed = 1200; }
    }
    public void Decelerate(float amount)
    {
        speed -= amount;
        if (speed < 1) { speed = 1; }
    }

    public void LockingOnTarget(Player target)
    {
        targetAngle = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();
        Rotation = targetAngle;
        isLockedOn = true;
    }
}
