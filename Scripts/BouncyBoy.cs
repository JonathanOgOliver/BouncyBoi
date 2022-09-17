using Godot;
using System;

public class BouncyBoy : RigidBody2D
{
    [Export] float _MinSpeed = 1;

    [Export] NodePath _RayCastPath;
    RayCast2D _RayCast;

    [Export] NodePath _CameraPath;
    Camera2D _Camera;

    Vector2 _startPosition;

    public Cannon cannon;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _RayCast = GetNode<RayCast2D>(_RayCastPath);
        _Camera = GetNode<Camera2D>(_CameraPath);
        _startPosition = Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var viewPortRect = GetViewportRect();
        float distanceToGround = GlobalPosition.y;
        float zoomLevel = -(distanceToGround / viewPortRect.Size.y) + 0.5f;
        zoomLevel = Mathf.Max(zoomLevel, 1.1f);
        float cameraYPos = distanceToGround * 0.5f - viewPortRect.Size.y * 0.25f;
        cameraYPos = Mathf.Min(cameraYPos, -viewPortRect.Size.y * zoomLevel * 0.5f);
        _Camera.GlobalPosition = new Vector2(GlobalPosition.x, cameraYPos);
        _Camera.Zoom = new Vector2(zoomLevel, zoomLevel);

        Score.Instance.CurrentScore = (int)(GlobalPosition.x - _startPosition.x);
        _RayCast.GlobalRotation = 0;
        if (LinearVelocity.Length() <= _MinSpeed && _RayCast.IsColliding())
        {
            LinearVelocity = Vector2.Zero;
            InfiniteScrollingBackground.Instance.FocusedObjectPath = cannon.GetPath();
            QueueFree();
        }
    }
}
