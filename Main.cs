using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    public override void _Ready()
    {
        // Hide the game over screen
        GetNode<Control>("Hud/GameOverScreen").Hide();
    }

    private void OnMobTimerTimeout()
    {
        // Create a new mob instance
        Mob mob = MobScene.Instantiate<Mob>();

        // Choose a random location on Path2D and give it a random offset
        var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        // Get the player's position
        var playerPosition = GetNode<Player>("Player").Position;

        // Initialize the mob
        mob.Initialize(mobSpawnLocation.Position, playerPosition);

        // Add the mob to the scene
        AddChild(mob);

        mob.Squashed += GetNode<ScoreLabel>("Hud/ScoreLabel").OnMobSquashed;
    }

    private void OnPlayerHit()
    {
        GetNode<Control>("Hud/GameOverScreen").Show();
        GetNode<Timer>("MobTimer").Stop();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && GetNode<Control>("Hud/GameOverScreen").Visible)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
