using Godot;
using System;

public class BouncyBoy : RigidBody2D
{
    [Export] float _MinSpeed = 1;

    [Export] NodePath _RayCastPath;
    RayCast2D _RayCast;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _RayCast = GetNode<RayCast2D>(_RayCastPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _RayCast.GlobalRotation = 0;
        if(LinearVelocity.Length() <= _MinSpeed && _RayCast.IsColliding())
        {
            LinearVelocity = Vector2.Zero;
            QueueFree();
        }
    }
}
