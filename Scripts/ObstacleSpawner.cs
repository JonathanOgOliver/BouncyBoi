using Godot;
using System;
using System.Collections.Generic;

public class ObstacleSpawner : Node2D
{
    [Export] PackedScene ObstacleScene;
    [Export] float minDistanceInterval = 100;
    [Export] float maxDistanceInterval = 2000;
    [Export] float nextInterval;
    [Export] float placementOffset = 1000;
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
    private float RandomInterval => (float)GD.RandRange(minDistanceInterval, maxDistanceInterval);


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
                nextInterval = BouncyBoy.Current.GlobalPosition.x + RandomInterval;
            }
            if(BouncyBoy.Current.GlobalPosition.x >= nextInterval){
                SpawnObstacle();
                nextInterval = BouncyBoy.Current.GlobalPosition.x + RandomInterval;
            }
            if(GetFarthestObject != null && BouncyBoy.Current.GlobalPosition.x-killDistance >= GetFarthestObject.GlobalPosition.x){
                GetFarthestObject.QueueFree();
                farthestObject = null;
            }
        }else{
            objects.Clear();
        }

    }

    private void SpawnObstacle(){
        GD.Randomize();

        float xPos = (BouncyBoy.Current.Camera.GlobalPosition.x+((GetViewportRect().Size.x*BouncyBoy.Current.Camera.Zoom.x)/2f))+placementOffset;
        float yPos = (float)GD.RandRange(2, -GetViewportRect().Size.y*BouncyBoy.Current.Camera.Zoom.y);
        float rot = (float)GD.RandRange(0, 360);

        Node2D newBoi = ObstacleScene.Instance<Node2D>();
        newBoi.GlobalPosition = new Vector2(xPos, yPos);
        newBoi.RotationDegrees = rot;

        AddChild(newBoi);
        objects.Enqueue(newBoi);
    }
}
