using Godot;
using System;

public class Score : Node
{
    private int _currentScore;
    public int CurrentScore { get => _currentScore; set { _currentScore = value; if (value > HighScore) HighScore = value; } }
    public int HighScore { get; private set; }
    public static Score Instance { get; private set; }

    [Export] NodePath _ScoreLabelPath;
    Label _ScoreLabel;
    [Export] NodePath _HighScoreLabelPath;
    Label _HighScoreLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
        _ScoreLabel = GetNode<Label>(_ScoreLabelPath);
        _HighScoreLabel = GetNode<Label>(_HighScoreLabelPath);

        var saveData = new File();
        if (saveData.FileExists("user://Highscore.bounce"))
        {
            saveData.Open("user://Highscore.bounce", File.ModeFlags.Read);
            HighScore = int.Parse(saveData.GetAsText());
            saveData.Close();
        }
    }

    public override void _ExitTree()
    {
        var saveData = new File();
        saveData.Open("user://Highscore.bounce", File.ModeFlags.Write);
        saveData.StoreString(HighScore.ToString());
        saveData.Flush();
        saveData.Close();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _ScoreLabel.Text = CurrentScore.ToString();
        _HighScoreLabel.Text = "Highscore: " + HighScore.ToString();
    }
}
