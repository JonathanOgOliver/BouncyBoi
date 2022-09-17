using Godot;
using System;

public class InfiniteScrollingBackground : Node2D
{
    [Export] private Texture backgroundTexture;

    private NodePath focusedObjectPath;
    [Export] public NodePath FocusedObjectPath { get{return focusedObjectPath;} set{focusedObjectPath = value; focusedObject = null;} }
    private Node2D focusedObject;

    [Export(PropertyHint.Range, "1,9999,")] private int amountAheadAndBehind = 1;

    private Sprite[] sprites;

    public static InfiniteScrollingBackground Instance { get; private set; }


    private int SpriteCount => 1+amountAheadAndBehind*2;
    private int TextureWidth => backgroundTexture.GetWidth();
    private int TextureHeight => backgroundTexture.GetHeight();

    private float yPos;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public override void _Ready()
    {
        if(CheckDependencies()){
            yPos = focusedObject.Position.y;
            GenerateSprites();
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if(CheckDependencies()){
            //TODO: Only run when a new %TextureWidth is passed.
            Vector2 startPos = new Vector2(focusedObject.Position.x-focusedObject.Position.x%TextureWidth-TextureWidth*amountAheadAndBehind, yPos);
            for(int i = 0; i < SpriteCount; i++){
                sprites[i].Position = new Vector2(startPos.x+TextureWidth*i, yPos);
            }
        }
    }

    private void GenerateSprites(){
        sprites = new Sprite[SpriteCount];
        Vector2 startPos = new Vector2(focusedObject.Position.x+focusedObject.Position.x%TextureWidth-TextureWidth*amountAheadAndBehind, yPos);

        for(int i = 0; i < SpriteCount; i++){
            sprites[i] = new Sprite();
            sprites[i].Texture = backgroundTexture;
            sprites[i].Position = new Vector2(startPos.x+TextureWidth*i, yPos);
            AddChild(sprites[i]);
        }
    }

    private bool CheckDependencies(){
        bool passed = true;
        if(backgroundTexture == null){
            GD.PushError("The node: " + Name + " is missing a background texture");
            passed = false;
        }
        if(focusedObjectPath == null){
            GD.PushError("The node: " + Name + " is missing a focused object");
            passed = false;
        }else if(focusedObject == null){
            focusedObject = GetNode<Node2D>(focusedObjectPath);
        }
        return passed;
    }
}
