using Godot;
using System;

public partial class Player : CharacterBody3D
{
    // Player speed (m/s)
    [Export]
    public int Speed { get; set; } = 14;

    // Downward acceleration (m/s^2)
    [Export]
    public int FallAcceleration { get; set; } = 75;

    // Jump impulse (m/s)
    [Export]
    public int JumpImpulse { get; set; } = 30;

    // Bounce impulse (m/s)
    [Export]
    public int BounceImpulse { get; set; } = 16;

    // Godmode: Player is invincible when true
    [Export]
    public bool Godmode { get; set; } = false;

    private Vector3 _targetVelocity = Vector3.Zero;

    [Signal]
    public delegate void HitEventHandler();

    public override void _PhysicsProcess(double delta)
    {
        var direction = GetDirection();

        if (direction != Vector3.Zero)
        {
            // Rotate the player to face the direction they are moving in
            GetNode<Node3D>("Pivot").LookAt(
                Position + direction,
                Vector3.Up
            );
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
        }
        else
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
        }

        // Ground Velocity
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        // Vertical Velocity
        if (!IsOnFloor())
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y += JumpImpulse;
        }

        // Apply velocity
        Velocity = _targetVelocity;
        MoveAndSlide();

        var pivot = GetNode<Node3D>("Pivot");
        pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);        
    }


    /// <summary>
    /// Returns the direction the player is moving in.
    ///
    /// <br/>
    /// This checks if the player is pressing any of the movement keys and returns
    /// a normalized vector or a Vector3.Zero, representing the direction(s) the player is moving in.
    /// </summary>
    private Vector3 GetDirection()
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_down"))
        {
            direction.Z += 1.0f;
        }

        if (Input.IsActionPressed("move_up"))
        {
            direction.Z -= 1.0f;
        }

        var slideCollisionCount = GetSlideCollisionCount();

        for (int i = 0; i < slideCollisionCount; i++)
        {
            KinematicCollision3D collision = GetSlideCollision(i);

            if (collision.GetCollider() is Mob mob)
            {
                bool isHittingFromAbove = Vector3.Up.Dot(collision.GetNormal()) > 0.1f;

                if (isHittingFromAbove)
                {
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;
                }
            }
        }

        return direction == Vector3.Zero ? direction : direction.Normalized();
    }

    private void Die()
    {
        if (Godmode) return;
        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    private void OnMobDetectorBodyEntered(Node3D body)
    {
        Die();
    }
}
