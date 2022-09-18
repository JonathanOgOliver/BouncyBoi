using Godot;
using System;
using System.Collections.Generic;

public class ObstacleSpawner : Node2D
{
    [Export] PackedScene ObstacleScene;
    [Export] float minTimeInterval = 0;
    [Export] float maxTimeInterval = 1;
    [Export] float placementOffset = 100;
    [Export] float killDistance = 100;
    private Queue<Node2D> objects = new Queue<Node2D>();
    
    private bool isStarted = false;

    private Node2D farthestObject;
    private Node2D GetFarthestObject { 
        get
        {
            if(farthestObject == null && objects.Count != 0)
            {
                farthestObject = objects.Dequeue();
            }
            return farthestObject;
        } 
    }


    public override void _Ready()
    {
        base._Ready();
    }
    public override void _Process(float delta)
    {
        base._Process(delta);
        if(IsInstanceValid(BouncyBoy.Current) ){
            if(!isStarted){
                isStarted = true;
                float time = (float)GD.RandRange(minTimeInterval, maxTimeInterval);
                GD.Print(time);
                GetTree().CreateTimer(time, false).Connect("timeout", this, "TimerCallback");
            }
            if(GetFarthestObject != null && (Position-BouncyBoy.Current.Position).Length() > killDistance){
                GetFarthestObject.QueueFree();
            }
        }

    }

    public void TimerCallback(){
        if(IsInstanceValid(BouncyBoy.Current)){
            GD.Randomize();

            float xPos = (BouncyBoy.Current.Camera.Position.x+((GetViewportRect().Size.x*BouncyBoy.Current.Camera.Zoom.x)/2f))+placementOffset;
            float yPos = (float)GD.RandRange(2, GetViewportRect().Size.y*BouncyBoy.Current.Camera.Zoom.y);
            float rot = (float)GD.RandRange(0, 360);

            Node2D newBoi = ObstacleScene.Instance<Node2D>();
            newBoi.Position = new Vector2(xPos, yPos);
            newBoi.RotationDegrees = rot;

            AddChild(newBoi);
            
            float time = (float)GD.RandRange(minTimeInterval, maxTimeInterval);
            GD.Print(time);

            GetTree().CreateTimer(time, false).Connect("timeout", this, "TimerCallback");
        }else{
            objects.Clear();
        }
    }
}
