using Godot;
using System;

public class Trampoline : Area2D
{
    [Export]
    public Vector2 Strength { get; private set; }
    [Export]  NodePath _AudioPlayerPath;
    AudioStreamPlayer _AudioPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _AudioPlayer = GetNode<AudioStreamPlayer>(_AudioPlayerPath);

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
            Vector2 newVel = new Vector2(bb.LinearVelocity.x, -bb.LinearVelocity.y).Rotated(GlobalRotation) * Strength;
            bb.LinearVelocity = new Vector2(Mathf.Abs(newVel.x), -Mathf.Abs(newVel.y));
            _AudioPlayer.Play();
        }
    }

}
