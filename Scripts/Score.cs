using Godot;
using System;

public class Score : Label
{
    private int _currentScore;
    public int CurrentScore { get => CurrentScore; set { _currentScore = value; if (value > HighScore) HighScore = CurrentScore; } }
    public int HighScore { get; private set; }
    public static Score Instance { get; private set; }

    [Export] NodePath _ScoreLabelPath;
    Label _ScoreLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
        _ScoreLabel = GetNode<Label>(_ScoreLabelPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _ScoreLabel.Text = CurrentScore.ToString();
    }
}
