using Godot;
using System;

public class BouncyBoy : RigidBody2D
{
    [Export] float _MinSpeed = 1;
    [Export] float _MaxCameraY = 5000;
    [Export] float _SurgeSpeeed = 10;
    [Export] float _SurgeAngle = 30;

    [Export] NodePath _RayCastPath;
    RayCast2D _RayCast;

    [Export] NodePath _CameraPath;
    Camera2D _Camera;
    public Camera2D Camera => _Camera;

    Vector2 _startPosition;

    public Cannon cannon;

    public static BouncyBoy Current { get; private set; }
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Current = this;
        _RayCast = GetNode<RayCast2D>(_RayCastPath);
        _Camera = GetNode<Camera2D>(_CameraPath);
        _startPosition = Position;
    }

    public override void _ExitTree()
    {
        Current = null;
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var viewPortRect = GetViewportRect();
        float distanceToGround = GlobalPosition.y;
        float maxZoomLevel = Mathf.Abs(_MaxCameraY) / viewPortRect.Size.y;
        float zoomLevel = -(distanceToGround / viewPortRect.Size.y) + 0.5f;
        zoomLevel = Mathf.Clamp(zoomLevel, 1.1f, maxZoomLevel);
        float cameraYPos = distanceToGround * 0.5f - viewPortRect.Size.y * 0.25f;
        cameraYPos = Mathf.Min(cameraYPos, -viewPortRect.Size.y * zoomLevel * 0.5f);
        cameraYPos = Mathf.Max(cameraYPos, _MaxCameraY * 0.5f);
        _Camera.GlobalPosition = new Vector2(GlobalPosition.x, cameraYPos);
        _Camera.Zoom = new Vector2(zoomLevel, zoomLevel);

        Score.Instance.CurrentScore = (int)(GlobalPosition.x - _startPosition.x);
        _RayCast.GlobalRotation = 0;
        bool onGround = _RayCast.IsColliding();

        if (LinearVelocity.Length() <= _MinSpeed && onGround)
        {
            LinearVelocity = Vector2.Zero;
            InfiniteScrollingBackground.focusedObject = cannon;
            QueueFree();
        }

        if(Input.IsActionPressed("Fire") && !onGround)
        {
            LinearVelocity = new Vector2(_SurgeSpeeed, 0).Rotated(Mathf.Deg2Rad(_SurgeAngle));
        }
    }
}
