using Godot;
using System;

public class Cannon : Node2D
{
    [Export] float _Strength = 1000;
    [Export] float _StartAngle = -15;
    [Export] float _RotationSpeed = 5;
    [Export] float _MinAngle = 0;
    [Export] float _MaxAngle = 90;

    [Export] NodePath _OutputPath;
    Node2D _Output;

    [Export] NodePath _StartOfBarrelPath;
    Node2D _StartOfBarrel;

    [Export] NodePath _CameraPath;
    Camera2D _Camera;

    [Export] PackedScene _BouncyBoy;



    BouncyBoy _currentBoy;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InfiniteScrollingBackground.Instance.FocusedObjectPath = this.GetPath();
        _Output = GetNode<Node2D>(_OutputPath);
        _StartOfBarrel = GetNode<Node2D>(_StartOfBarrelPath);
        _Camera = GetNode<Camera2D>(_CameraPath);

        _StartOfBarrel.RotationDegrees = _StartAngle;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(Input.IsActionPressed("Aim_up"))
        {
            _StartOfBarrel.RotationDegrees -= _RotationSpeed * delta;
        }
        if(Input.IsActionPressed("Aim_down"))
        {
            _StartOfBarrel.RotationDegrees += _RotationSpeed * delta;
        }

        _StartOfBarrel.RotationDegrees = Mathf.Clamp(_StartOfBarrel.RotationDegrees, _MinAngle, _MaxAngle);

        bool boyExists = IsInstanceValid(_currentBoy);
        _Camera.Current = !boyExists;

        if (Input.IsActionJustPressed("Fire") && !boyExists)
        {
            _currentBoy = _BouncyBoy.Instance<BouncyBoy>();
            _currentBoy.cannon = this;
            _currentBoy.GlobalPosition = _Output.GlobalPosition;
            GetTree().Root.AddChild(_currentBoy);

            Vector2 direction = (_Output.GlobalPosition - _StartOfBarrel.GlobalPosition).Normalized();
            _currentBoy.LinearVelocity = direction * _Strength;
            InfiniteScrollingBackground.Instance.FocusedObjectPath = _currentBoy.GetPath();

        }
    }
}
