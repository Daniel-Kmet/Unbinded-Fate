extends Node2D

# Preload the projectile scene (ensure the path is correct)
@export var projectile_scene: PackedScene

# Optional: define a node as the muzzle where the projectile will spawn
@onready var muzzle = $Muzzle

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("fire"):
		shoot_projectile()

func shoot_projectile() -> void:
	# Create a new instance of the projectile
	var projectile = projectile_scene.instantiate()
	
	# Set the projectile's starting position at the muzzle or player's position
	projectile.global_position = muzzle.global_position
	
	# Calculate the direction for the projectile (example: toward the mouse)
	var target_position = get_global_mouse_position()
	projectile.direction = (target_position - projectile.global_position).normalized()
	
	# Add the projectile to the scene tree (it could be added as a sibling or to a dedicated container)
	get_tree().current_scene.add_child(projectile)
