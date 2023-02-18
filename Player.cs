using Godot;

public partial class Player : Area2D {
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int speed = 400; // How fast the player will move (pixels/sec).

	public Vector2 screenSize; // Size of the game window.

	public override void _Ready() {
		screenSize = GetViewportRect().Size;
		Hide();
	}

	public override void _Process(double delta) {
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right")) velocity.X += 1;
		if (Input.IsActionPressed("move_left")) velocity.X -= 1;
		if (Input.IsActionPressed("move_down")) velocity.Y += 1;
		if (Input.IsActionPressed("move_up")) velocity.Y -= 1;

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0) {
			velocity = velocity.Normalized() * speed;
			animatedSprite.Play();
		} else animatedSprite.Stop();

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, screenSize.Y)
		);

		if (velocity.X != 0) {
			animatedSprite.Animation = "walk";
			// See the note below about boolean assignment.
			animatedSprite.FlipH = velocity.X < 0;
			animatedSprite.FlipV = false;
		} else if (velocity.Y != 0) {
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.Y > 0;
		}
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	public void _on_body_entered(PhysicsBody2D body)
	{
		Hide(); // Player disappears after being hit.

		EmitSignal(nameof(Hit));
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}
}
