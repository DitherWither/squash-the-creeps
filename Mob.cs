using Godot;
using System;

public partial class Mob : CharacterBody3D
{
    // Minimum and maximum speeds (m/s)

    [Export]
    public int MinSpeed { get; set; } = 10;

    [Export]
    public int MaxSpeed { get; set; } = 18;

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    [Signal]
    public delegate void SquashedEventHandler();

    /// <summary>
    /// Initializes the mob.
    /// </summary>
    public void Initialize(Vector3 startPosition, Vector3 playerPosition)
    {
        // Look at the player
        LookAtFromPosition(startPosition, playerPosition, Vector3.Up);

        // Set a random rotation in the Y axis
        // so that the mob doesn't always face the player
        RotateY((float)GD.RandRange(-Mathf.Pi / 4.0f, Mathf.Pi / 4.0f));

        // Set a random speed
        int randomSpeed = (int)GD.RandRange(MinSpeed, MaxSpeed);

        // Set the velocity
        Velocity = Vector3.Forward * randomSpeed;

        // Rotate based on the rotation in the Y axis
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

        GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = randomSpeed / MinSpeed;
    }

    /// <summary>
    /// Kills the mob when it exits the screen.
    /// </summary>
    private void OnVisibilityNotifierScreenExited()
    {
        QueueFree();
    }

    public void Squash()
    {
        EmitSignal(SignalName.Squashed);
        QueueFree();
    }
}
