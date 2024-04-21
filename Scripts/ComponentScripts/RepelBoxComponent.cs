using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class RepelBoxComponent : Area2D
{
    //the movement component the repel will send push messages to;
    [Export] MovementComponent movementComponent;

    //the collision shape of the repel component
	CollisionShape2D collisionShape;
    //a list of the other repel components this one is in contact with
	List<RepelBoxComponent> targetsInShoveRange = new List<RepelBoxComponent>();

    //how fast the parent is pushed when in contact with other repel component (in pixels per second)
    float pushSpeed = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetPushSpeed(float newPushSpeed)
    {
        pushSpeed = newPushSpeed;
    }

    public Vector2 GeneratePushVelocity()
    {
        //create a new velocity
        Vector2 newVelocity = new Vector2();

        foreach (RepelBoxComponent target in targetsInShoveRange)
        {
            //check if the component is valid (ie, target hasnt died), remove if not
            if (IsInstanceValid(target))
            {
                //add a velocity *away* from the target component
                newVelocity -= GlobalPosition.DirectionTo(target.GlobalPosition) * pushSpeed;
            }
            else
            {
                RemoveFromRepelList(target);
            }
        }

        return newVelocity;
    }

    //add the other component to the shove list if it is a repel component
    public void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("RepelBoxComponent"))
		{
            RepelBoxComponent otherComponent = (RepelBoxComponent)area;
            AddToRepelList(otherComponent);
            
		}
	}

    //remove the other component from the shove list if it was a repel component
    public void OnAreaExited(Area2D area)
    {
        if (area.IsInGroup("RepelBoxComponent"))
        {
            RepelBoxComponent otherComponent = (RepelBoxComponent)area;
            RemoveFromRepelList(otherComponent);
        }
    }

	public void AddToRepelList(RepelBoxComponent target)
	{
        targetsInShoveRange.Add(target);
    }

	public void RemoveFromRepelList(RepelBoxComponent target)
	{
        //create a temporary list to add the shove list elements from
        List<RepelBoxComponent> tempList = new List<RepelBoxComponent>();

        //check if the target repelbox is in the shove list and add that element to the temp list
        foreach (RepelBoxComponent checkedTarget in targetsInShoveRange)
        {
            if (checkedTarget == target)
            {
                tempList.Add(checkedTarget);
            }
        }

        //delete all valid elements from the shove list
        foreach (RepelBoxComponent removeTarget in tempList)
        {
            targetsInShoveRange.Remove(removeTarget);
        }
    }
}
