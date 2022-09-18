using Godot;
using System;
using System.Collections.Generic;

public class InfiniteScrollingBackground : Node2D
{
    [Export] private Texture backgroundTexture;

    public static Node2D focusedObject { get; set; }
    private List<Sprite> sprites = new List<Sprite>();

    public static InfiniteScrollingBackground Instance { get; private set; }

    private int SpriteCount => Mathf.CeilToInt(IsInstanceValid(BouncyBoy.Current) ? (GetViewportRect().Size.x * BouncyBoy.Current.Camera.Zoom.x) / TextureWidth : GetViewportRect().Size.x / TextureWidth) + 2;
    private int TextureWidth => backgroundTexture.GetWidth();
    private int TextureHeight => backgroundTexture.GetHeight();

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public override void _Ready()
    {
        if (CheckDependencies())
        {
            GenerateSprites();
        }
    }

    public override void _Process(float delta)
    {
        if (sprites.Count < SpriteCount)
            GenerateSprites();

        if (CheckDependencies())
        {
            Vector2 startPos = new Vector2(focusedObject.GlobalPosition.x - focusedObject.GlobalPosition.x % TextureWidth - TextureWidth * Mathf.FloorToInt(sprites.Count * 0.5f) + TextureWidth, GlobalPosition.y);
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].GlobalPosition = new Vector2(startPos.x + TextureWidth * i, GlobalPosition.y);
            }
        }
    }

    private void GenerateSprites()
    {
        for (int i = sprites.Count; i < SpriteCount; i++)
        {
            sprites.Add(new Sprite());
            sprites[i].Texture = backgroundTexture;
            AddChild(sprites[i]);
        }
    }

    private bool CheckDependencies()
    {
        bool passed = true;
        if (backgroundTexture == null)
        {
            GD.PushError("The node: " + Name + " is missing a background texture");
            passed = false;
        }else if(!IsInstanceValid(focusedObject))
        {
            passed = false;
        }
        return passed;
    }
}
