using Godot;
using System;

public class BouncyBoy : RigidBody2D
{
    [Export] float _RestartTime = 2;
    [Export] float _MaxCameraY = 5000;
    [Export] float _SurgeSpeeed = 10;

    [Export] NodePath _RayCastPath;
    RayCast2D _RayCast;

    [Export] NodePath _CameraPath;
    Camera2D _Camera;
    public Camera2D Camera => _Camera;

    [Export] NodePath _RotationMarkerPath;
    Node2D _RotationMarker;

    [Export]  NodePath _AudioPlayerPath;
    AudioStreamPlayer _AudioPlayer;
    [Export] AudioStreamSample[] _GroundSamples;

    Vector2 _startPosition;

    float _resetTimer;

    public Cannon cannon;

    public static BouncyBoy Current { get; private set; }

    bool _HasPlayedSound = false;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Current = this;
        _RayCast = GetNode<RayCast2D>(_RayCastPath);
        _Camera = GetNode<Camera2D>(_CameraPath);
        _RotationMarker = GetNode<Node2D>(_RotationMarkerPath);
        _AudioPlayer = GetNode<AudioStreamPlayer>(_AudioPlayerPath);
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

        if (onGround){
            if(!_HasPlayedSound){
                GD.Randomize();
                _AudioPlayer.Stream = _GroundSamples[(int)GD.RandRange(0, _GroundSamples.Length)];
                _AudioPlayer.Play();
                _HasPlayedSound = true;
            }
            _resetTimer -= delta;

        }
        else{
            _resetTimer = _RestartTime;
            _HasPlayedSound = false;
        }

        if (onGround && _resetTimer <= 0)
        {
            LinearVelocity = Vector2.Zero;
            InfiniteScrollingBackground.focusedObject = cannon;
            Score.Instance.Save();
            QueueFree();
        }

        if (Input.IsActionPressed("Fire"))
        {
            float currentRotation = Mathf.Rad2Deg(LinearVelocity.Angle());
            if (currentRotation <= 90 && currentRotation >= -90)
            {
                LinearVelocity = LinearVelocity.Rotated(_SurgeSpeeed * delta);
            }
        }

        _RotationMarker.GlobalRotation = LinearVelocity.Angle();
    }
}
