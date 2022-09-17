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

    [Export] PackedScene _BouncyBoy;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _Output = GetNode<Node2D>(_OutputPath);
        _StartOfBarrel = GetNode<Node2D>(_StartOfBarrelPath);

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

        if (Input.IsActionJustPressed("Fire"))
        {
            var boy = _BouncyBoy.Instance<RigidBody2D>();
            GetTree().Root.AddChild(boy);
            boy.GlobalPosition = _Output.GlobalPosition;

            Vector2 direction = (_Output.GlobalPosition - _StartOfBarrel.GlobalPosition).Normalized();
            boy.LinearVelocity = direction * _Strength;
        }
    }

}
