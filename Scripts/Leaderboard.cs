using Godot;
using System;
using System.Collections.Generic;

public class Leaderboard : Control
{
    public static Leaderboard Instance { get; private set; }

    [Export] NodePath _LeaderboardConnectionPath;
    Node2D _LeaderboardConnection;

    [Export] NodePath _RestartButtonPath;
    Button _RestartButton;

    [Export] NodePath _LeaderboardVBoxPath;
    VBoxContainer _LeaderboardVBox;

    [Export] PackedScene _ScoreEntryScene;

    [Export] NodePath _NewHighscoreSectionPath;
    Control _NewHighscoreSection;

    [Export] NodePath _NameBoxPath;
    LineEdit _NameBox;

    [Export] NodePath _SubmitButtonPath;
    Button _SubmitButton;

    int lastSubmittedHighscore;
    string lastName;

    List<Node> _scoreEntries = new List<Node>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
        Visible = false;
        _LeaderboardConnection = GetNode<Node2D>(_LeaderboardConnectionPath);
        _RestartButton = GetNode<Button>(_RestartButtonPath);
        _LeaderboardVBox = GetNode<VBoxContainer>(_LeaderboardVBoxPath);
        _NameBox = GetNode<LineEdit>(_NameBoxPath);
        _SubmitButton = GetNode<Button>(_SubmitButtonPath);
        _NewHighscoreSection = GetNode<Control>(_NewHighscoreSectionPath);

        _RestartButton.Connect("pressed", this, nameof(Restart));
        _SubmitButton.Connect("pressed", this, nameof(Submit));
        _LeaderboardConnection.Connect("name_gotten", this, nameof(NameGotten));
        _LeaderboardConnection.Connect("leaderboard_gotten", this, nameof(LeaderboardGotten));
        _LeaderboardConnection.Connect("score_uploaded", this, nameof(ScoreUploaded));
    }

    private void ScoreUploaded()
    {
        _LeaderboardConnection.Call("_get_leaderboards");
    }

    private void LeaderboardGotten(object[] ranks, object[] names, object[] scores)
    {
        foreach (var entry in _scoreEntries)
            entry.QueueFree();

        for (int i = 0; i < ranks.Length; i++)
        {
            var entry = _ScoreEntryScene.Instance();
            _LeaderboardVBox.AddChild(entry);
            entry.GetNode<Label>("HBoxContainer/Nr").Text = ranks[i].ToString();
            entry.GetNode<Label>("HBoxContainer/Name").Text = names[i].ToString();
            entry.GetNode<Label>("HBoxContainer/Score").Text = scores[i].ToString();
            _scoreEntries.Add(entry);
        }
    }

    private void Submit()
    {
        if (_NameBox.Text.Trim() != lastName)
        {
            lastName = _NameBox.Text.Trim();
            _LeaderboardConnection.Call("_change_player_name", _NameBox.Text.Trim());
        }

        _LeaderboardConnection.Call("_upload_score", Score.Instance.HighScore);
        lastSubmittedHighscore = Score.Instance.HighScore;
        _NewHighscoreSection.Visible = false;
    }

    public void NameGotten(string name)
    {
        _NameBox.Text = name;
    }

    public new void Show()
    {
        if (Visible)
            return;

        Visible = true;
        Score.Instance.Save();
        _LeaderboardConnection.Call("_get_player_name");
        _LeaderboardConnection.Call("_get_leaderboards");
        _NewHighscoreSection.Visible = Score.Instance.HighScore > lastSubmittedHighscore;
    }

    public void Restart()
    {
        Visible = false;
        BouncyBoy.Current.QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _SubmitButton.Disabled = _NameBox.Text.Trim().Length == 0;
    }
}
