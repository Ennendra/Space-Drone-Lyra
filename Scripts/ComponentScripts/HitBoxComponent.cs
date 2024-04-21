using Godot;
using System;
using System.Collections.Generic;


//enum which will dictate how the projectile will behave when hitting targets
//BASIC -- Projectile will hit target and disappear after. A standard bullet projectile
//HITONCE -- Projectile can hit multiple targets, but will never hit the same enemy twice during its lifecycle
//HITCOOLDOWN -- Projectile can hit multiple targets, and can hit the same enemy after a short preset period.
//NOHIT -- will not run anything when hitting targets. Useful for 'initial' projectiles that will detonate on a timer instead
public enum ProjectileHitBehaviour
{
    BASIC,
    HITONCE,
    HITCOOLDOWN,
    NOHIT
}

//A class containing data to help with the ProjectileHitBehaviour.
//Contains: the enemy hit, whether the hit can reset (ie, it is not a HITONCE projectile) and the time (in seconds) it takes before it can hit the enemy again, if applicable
internal class TargetHitTimer
{
    internal HurtBoxComponent target;
    internal float hitCooldown;
    internal bool canHitReset;
}

public partial class HitBoxComponent : Area2D
{

    //stats that can be altered by the weapon
    public double damage = 3;
    public DamageType damageType;


    public float distanceToDeath = 500.0f; //how far in pixels the projectile can fly before detonating
    
    //Determines how the projectile will operate when it hits enemies, see the enum above
    [Export] ProjectileHitBehaviour hitBehaviour = ProjectileHitBehaviour.BASIC;
    //Lists which help determine whether a target has already been hit and don't want to be hit again
    List<TargetHitTimer> hitList = new List<TargetHitTimer>();

    Node2D parentNode;

    //a reference to the global script (used for generating damage indicators)
    GlobalScript globals;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = GetNode<GlobalScript>("/root/Globals");


        parentNode = GetParent<Node2D>();
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //checks if this projectile's behaviour hits multiple enemies with cooldowns
        if (hitBehaviour == ProjectileHitBehaviour.HITCOOLDOWN)
        {
            //processes each hitTimer and removes it if the cooldown has ended
            foreach (TargetHitTimer tempTimer in hitList)
            {
                //process the timer
                tempTimer.hitCooldown -= (float)delta;
                //check if the hurtbox still exists (ie, target isn't dead)
                if (IsInstanceValid(tempTimer.target))
                {
                    //check if we're due to deal damage again
                    if (tempTimer.hitCooldown <= 0.0f)
                    {
                        DealDamage(tempTimer.target);
                        tempTimer.hitCooldown = 1.0f;
                    }
                }
                else
                {
                    hitList.Remove(tempTimer);
                }
            }
        }
    }

    //Colliding with a hurtbox
    public void OnAreaEntered(Area2D area)
    {
        bool isStandardBulletDeath = true;
        //confirm that the area entered is a hurtbox
        if (area.IsInGroup("HurtBoxComponent"))
        {
            //cast the entered area as a hurtbox component variable
            HurtBoxComponent hitComponent = (HurtBoxComponent)area;

            //Deal damage depending on hitbehaviour
            switch (hitBehaviour)
            {
                //BASIC: Simply deal damage, since the projectile will be destroyed momentarily
                case ProjectileHitBehaviour.BASIC:
                    {
                        if (hitComponent.ADSLink == null)
                            {DealDamage(hitComponent);}
                        else
                        {
                            if (hitComponent.ADSLink.CanADSFire())
                            {
                                hitComponent.ADSLink.FireADSWeapon(parentNode);
                                isStandardBulletDeath = false;
                            }
                            else
                            {
                                DealDamage(hitComponent);
                            }
                            
                        }
                        
                        
                        break;
                    }
                //HITONCE: Deal damage if the enemy is not in the hitList, then add it to said list
                case ProjectileHitBehaviour.HITONCE:
                    {
                        if (!IsInHitList(hitComponent))
                        {
                            DealDamage(hitComponent);
                            AddToHitList(hitComponent, false);
                        }
                        break;
                    }
                //HITCOOLDOWN: Deal damage if the enemy is not in the hitList, then add to the list with a cooldown (which will remove them from the list in the _process function)
                case ProjectileHitBehaviour.HITCOOLDOWN:
                    {
                        if (!IsInHitList(hitComponent))
                        {
                            DealDamage(hitComponent);
                            AddToHitList(hitComponent, true);
                        }
                        break;
                    }
            }
        }

        //check if the behaviour is basic and destroy the projectile if so
        if (hitBehaviour == ProjectileHitBehaviour.BASIC)
        {
            if (isStandardBulletDeath)
                {parentNode.EmitSignal("emitParticle");}

            parentNode.EmitSignal("deathSignal");
        }

    }

    public void OnAreaExited(Area2D area)
    {
        //check if this is a cooldown-based attack and the target area is a hurtbox
        if (hitBehaviour == ProjectileHitBehaviour.HITCOOLDOWN && area.IsInGroup("HurtBoxComponent"))
        {
            //cast the entered area as a hurtbox component variable
            HurtBoxComponent hitComponent = (HurtBoxComponent)area;
            if (IsInHitList(hitComponent))
            {
                RemoveFromHitList(hitComponent);
            }
        }
    }

    //checks if the enemy in question is in the hitList (ie, they have been hit before)
    public bool IsInHitList(HurtBoxComponent target)
    {
        foreach (TargetHitTimer checkedTarget in hitList)
        {
            if (checkedTarget.target == target) return true;
        }
        return false;
    }

    //adds an enemy to the already hit list
    public void AddToHitList(HurtBoxComponent target, bool isHitCooldown)
    {
        TargetHitTimer addedEnemy = new TargetHitTimer();
        addedEnemy.target = target;
        addedEnemy.canHitReset = isHitCooldown;
        addedEnemy.hitCooldown = 1.0f;
        hitList.Add(addedEnemy);
    }

    public void RemoveFromHitList(HurtBoxComponent target)
    {
        //create a temporary list to add hitlist elements from
        List<TargetHitTimer> tempList = new List<TargetHitTimer>();

        //check if the target hurtbox is in the hitlist and add that element to the ttemp list
        foreach (TargetHitTimer checkedTarget in hitList)
        {
            if (checkedTarget.target == target)
            {
                tempList.Add(checkedTarget);
            }
        }

        //delete all valid elements from the hitlist
        foreach (TargetHitTimer removeTarget in tempList)
        {
            hitList.Remove(removeTarget);
        }

    }

    //deals damage and generates a damage indicator
    public void DealDamage(HurtBoxComponent hitTarget)
    {
        double damageDealt = hitTarget.TakeDamage(damage, damageType);
        globals.GenerateDamageIndicator(damageDealt, hitTarget.GlobalPosition);
    }
}
