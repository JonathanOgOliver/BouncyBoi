using Godot;
using System;

public class PlayerAbove : Control
{
    [Export] float _HeightToBeAbove = -1000;

    [Export] NodePath _LabelPath;
    Label _Label;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _Label = GetNode<Label>(_LabelPath);
        Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (IsInstanceValid(BouncyBoy.Current))
        {
            float boyY = BouncyBoy.Current.GlobalPosition.y;
            if (boyY <= _HeightToBeAbove)
            {
                Visible = true;
                _Label.Text = "+" + (int)(_HeightToBeAbove - boyY);
            }
            else
                Visible = false;
        }
        else
        {
            Visible = false;
        }
    }
}
