using Godot;
using System;

public partial class ArenaWall : StaticBody2D
{

    //the edges of the arena
    SegmentShape2D cShapeTop, cShapeLeft, cShapeRight, cShapeBottom;

    //the sprites to show the arena, as well as the animated tween parts
    Sprite2D cSpriteTop, cSpriteLeft, cSpriteRight, cSpriteBottom;
    Sprite2D cAnimSpriteTop, cAnimSpriteLeft, cAnimSpriteRight, cAnimSpriteBottom;

    Tween tweenTop, tweenLeft, tweenRight, tweenBottom;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        cShapeTop = GetNode<CollisionShape2D>("ColliderTop").Shape as SegmentShape2D;
        cShapeLeft = GetNode<CollisionShape2D>("ColliderLeft").Shape as SegmentShape2D;
        cShapeRight = GetNode<CollisionShape2D>("ColliderRight").Shape as SegmentShape2D;
        cShapeBottom = GetNode<CollisionShape2D>("ColliderBottom").Shape as SegmentShape2D;

        cSpriteTop = GetNode<Sprite2D>("ColliderTop/WallSprite");
        cSpriteLeft = GetNode<Sprite2D>("ColliderLeft/WallSprite");
        cSpriteRight = GetNode<Sprite2D>("ColliderRight/WallSprite");
        cSpriteBottom = GetNode<Sprite2D>("ColliderBottom/WallSprite");

        cAnimSpriteTop = GetNode<Sprite2D>("ColliderTop/WallSpriteAnim");
        cAnimSpriteLeft = GetNode<Sprite2D>("ColliderLeft/WallSpriteAnim");
        cAnimSpriteRight = GetNode<Sprite2D>("ColliderRight/WallSpriteAnim");
        cAnimSpriteBottom = GetNode<Sprite2D>("ColliderBottom/WallSpriteAnim");

    }

    public void SetArenaBounds(Vector2 center, Vector2 size)
    {
        //set the collider points
        cShapeTop.A = new Vector2(-size.X,-size.Y);
        cShapeTop.B = new Vector2(size.X, -size.Y);

        cShapeLeft.A = new Vector2(-size.X, -size.Y);
        cShapeLeft.B = new Vector2(-size.X, size.Y);

        cShapeRight.A = new Vector2(size.X, -size.Y);
        cShapeRight.B = new Vector2(size.X, size.Y);

        cShapeBottom.A = new Vector2(-size.X, size.Y);
        cShapeBottom.B = new Vector2(size.X, size.Y);

        //set the position of the sprite and spriteAnimated nodes
        cSpriteTop.Position = new Vector2(0,-size.Y);
        cSpriteLeft.Position = new Vector2(-size.X,0);
        cSpriteRight.Position = new Vector2(size.X,0);
        cSpriteBottom.Position = new Vector2(0,size.Y);

        

        //Set the scale of the sprite and animated nodes
        cSpriteTop.Scale = new Vector2(1,size.X / 5);
        cSpriteLeft.Scale = new Vector2(1, size.Y / 5);
        cSpriteRight.Scale = new Vector2(1, size.Y / 5);
        cSpriteBottom.Scale = new Vector2(1, size.X / 5);

        //call the function to set the animated nodes to their initial positions
        ResetAnimation();
    }

    public void ResetAnimNodeProperties()
    {
        //set the nodes' position and scales
        cAnimSpriteTop.Position = cSpriteTop.Position;
        cAnimSpriteLeft.Position = cSpriteLeft.Position;
        cAnimSpriteRight.Position = cSpriteRight.Position;
        cAnimSpriteBottom.Position = cSpriteBottom.Position;
        cAnimSpriteTop.Scale = cSpriteTop.Scale;
        cAnimSpriteLeft.Scale = cSpriteLeft.Scale;
        cAnimSpriteRight.Scale = cSpriteRight.Scale;
        cAnimSpriteBottom.Scale = cSpriteBottom.Scale;

        //set the animated nodes to be transparent
        //cAnimSpriteTop.Modulate = new Color(1, 1, 1, 0);
        //cAnimSpriteLeft.Modulate = new Color(1, 1, 1, 0);
        //cAnimSpriteRight.Modulate = new Color(1, 1, 1, 0);
        //cAnimSpriteBottom.Modulate = new Color(1, 1, 1, 0);

    }

    public void ResetAnimation()
    {
        //remove the previous tweens where applicable
        if (tweenTop != null) { tweenTop.Kill(); }
        if (tweenLeft != null) { tweenLeft.Kill(); }
        if (tweenRight != null) { tweenRight.Kill(); }
        if (tweenBottom != null) { tweenBottom.Kill(); }

        //reset the animated nodes' properties
        ResetAnimNodeProperties();

        //create the initial animation tweens
        tweenTop = CreateTween();
        tweenLeft = CreateTween();
        tweenRight = CreateTween();
        tweenBottom = CreateTween();
        tweenTop.SetParallel(false);
        tweenLeft.SetParallel(false);
        tweenRight.SetParallel(false);
        tweenBottom.SetParallel(false);

        //create a cubic animation style and set it to infinite loop
        tweenTop.SetTrans(Tween.TransitionType.Cubic);
        tweenTop.SetLoops();
        tweenLeft.SetTrans(Tween.TransitionType.Cubic);
        tweenLeft.SetLoops();
        tweenRight.SetTrans(Tween.TransitionType.Cubic);
        tweenRight.SetLoops();
        tweenBottom.SetTrans(Tween.TransitionType.Cubic);
        tweenBottom.SetLoops();

        double animLength = 0.5;

        tweenTop.TweenProperty(cAnimSpriteTop, "position", cAnimSpriteTop.Position + new Vector2(0, 7), animLength);
        tweenTop.TweenProperty(cAnimSpriteTop, "position", cAnimSpriteTop.Position, animLength);

        tweenLeft.TweenProperty(cAnimSpriteLeft, "position", cAnimSpriteLeft.Position + new Vector2(7, 0), animLength);
        tweenLeft.TweenProperty(cAnimSpriteLeft, "position", cAnimSpriteLeft.Position, animLength);

        tweenRight.TweenProperty(cAnimSpriteRight, "position", cAnimSpriteRight.Position - new Vector2(7, 0), animLength);
        tweenRight.TweenProperty(cAnimSpriteRight, "position", cAnimSpriteRight.Position, animLength);

        tweenBottom.TweenProperty(cAnimSpriteBottom, "position", cAnimSpriteBottom.Position - new Vector2(0, 7), animLength);
        tweenBottom.TweenProperty(cAnimSpriteBottom, "position", cAnimSpriteBottom.Position, animLength);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
