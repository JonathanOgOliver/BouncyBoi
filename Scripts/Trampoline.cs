using Godot;
using System;

public class Trampoline : Area2D
{
    [Export]
    public float Strength { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnBodyEntered));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    void OnBodyEntered(Node2D other)
    {
        if(other is BouncyBoy bb)
        {
            bb.LinearVelocity = new Vector2(bb.LinearVelocity.x, -bb.LinearVelocity.y * Strength);
        }
    }

}
