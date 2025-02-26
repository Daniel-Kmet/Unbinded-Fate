extends CharacterBody2D

# Speed of the projectile (pixels per second)
@export var speed: float = 500.0

# The direction the projectile will travel (should be set to a normalized vector)
@export var direction: Vector2 = Vector2.RIGHT

# Lifetime of the projectile in seconds (after which it is freed)
@export var lifetime: float = 2.0

var time_elapsed: float = 0.0

func _physics_process(delta: float) -> void:
	# Set the velocity property based on the normalized direction and speed
	velocity = direction.normalized() * speed

	# Move the projectile using the built-in move_and_slide()
	move_and_slide()

	# Update the time elapsed and free the projectile if lifetime is exceeded
	time_elapsed += delta
	if time_elapsed >= lifetime:
		queue_free()
