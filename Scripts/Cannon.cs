using Godot;
using System;

public class Cannon : Node2D
{
    [Export] float _Strength = 1000;
    [Export] float _StartAngle = -15;
    [Export] float _RotationSpeed = 5;
    [Export] float _MinAngle = 0;
    [Export] float _MaxAngle = 90;
    [Export] float _FireTime = 0.6f;
    [Export] float _ScaleSpeed = 1f;


    [Export] NodePath _OutputPath;
    Node2D _Output;

    [Export] NodePath _StartOfBarrelPath;
    Node2D _StartOfBarrel;

    [Export] NodePath _CameraPath;
    Camera2D _Camera;

    [Export] NodePath _FireSoundPath;
    AudioStreamPlayer2D _FireSound;

    [Export] NodePath _BeforeFireingPath;
    AudioStreamPlayer _BeforeFireing;

    [Export] NodePath _AfterFireingPath;
    AudioStreamPlayer _AfterFireing;

    [Export] PackedScene _BouncyBoy;

    BouncyBoy _currentBoy;

    float _TimeToFire = 0f;

    bool _hasSetAngle;
    int _angleAndSpeedDirection = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InfiniteScrollingBackground.focusedObject = this;
        _Output = GetNode<Node2D>(_OutputPath);
        _StartOfBarrel = GetNode<Node2D>(_StartOfBarrelPath);
        _Camera = GetNode<Camera2D>(_CameraPath);
        _FireSound = GetNode<AudioStreamPlayer2D>(_FireSoundPath);
        _BeforeFireing = GetNode<AudioStreamPlayer>(_BeforeFireingPath);
        _AfterFireing = GetNode<AudioStreamPlayer>(_AfterFireingPath);

        _StartOfBarrel.RotationDegrees = _StartAngle;

        _BeforeFireing.Play();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _StartOfBarrel.RotationDegrees = Mathf.Clamp(_StartOfBarrel.RotationDegrees, _MinAngle, _MaxAngle);

        bool boyExists = IsInstanceValid(_currentBoy);
        _Camera.Current = !boyExists;

        if (!boyExists)
        {
            if (_AfterFireing.Playing)
            {
                _AfterFireing.Stop();
                _BeforeFireing.Play();
            }

            if (!_hasSetAngle)
            {
                _StartOfBarrel.RotationDegrees += _RotationSpeed * delta * _angleAndSpeedDirection;
                if (_StartOfBarrel.RotationDegrees >= _MaxAngle && _angleAndSpeedDirection > 0)
                    _angleAndSpeedDirection = -1;
                else if (_StartOfBarrel.RotationDegrees <= _MinAngle && _angleAndSpeedDirection < 0)
                    _angleAndSpeedDirection = 1;

                if (Input.IsActionJustPressed("Fire"))
                {
                    _hasSetAngle = true;
                    _angleAndSpeedDirection = 1;
                }
            }
            else if(_TimeToFire <= 0)
            {
                _StartOfBarrel.Scale = new Vector2(_StartOfBarrel.Scale.x + _ScaleSpeed * delta * _angleAndSpeedDirection, _StartOfBarrel.Scale.y);
                if (_StartOfBarrel.Scale.x >= 1 && _angleAndSpeedDirection > 0)
                    _angleAndSpeedDirection = -1;
                else if (_StartOfBarrel.Scale.x <= 0.1f && _angleAndSpeedDirection < 0)
                    _angleAndSpeedDirection = 1;

                if (Input.IsActionJustPressed("Fire"))
                {
                    _FireSound.Play();
                    _TimeToFire = _FireTime;
                }
            }
        }


        if (_TimeToFire > 0)
        {
            _TimeToFire -= delta;
            if (_TimeToFire <= 0)
            {
                _currentBoy = _BouncyBoy.Instance<BouncyBoy>();
                _currentBoy.cannon = this;

                _currentBoy.GlobalPosition = _StartOfBarrel.GlobalPosition;
                GetTree().Root.AddChild(_currentBoy);
                InfiniteScrollingBackground.focusedObject = _currentBoy;

                Vector2 direction = (_Output.GlobalPosition - _StartOfBarrel.GlobalPosition).Normalized();
                _currentBoy.LinearVelocity = direction * _Strength * _StartOfBarrel.Scale.x;

                _AfterFireing.Play();
                _BeforeFireing.Stop();
                _hasSetAngle = false;
                _StartOfBarrel.Scale = new Vector2(1, _StartOfBarrel.Scale.y);
                _StartOfBarrel.RotationDegrees = (float)GD.RandRange(_MinAngle, _MaxAngle);
            }
        }
    }
}
