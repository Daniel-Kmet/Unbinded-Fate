extends Node2D

@export var chunk_width: int = 256
@export var chunk_height: int = 256
@export var tilemap_node_path: NodePath = "FloorTileMap"

func _ready() -> void:
	# Optional: If you want to randomize certain elements in this chunk,
	# do it here. For instance, randomly place some props or spawn objects.
	randomize_chunk_content()

func randomize_chunk_content() -> void:
	var tilemap = get_node(tilemap_node_path)
	if tilemap == null:
		return

	# Example: You could place random tiles if not pre-painted in the editor
	# Or randomly spawn a sprite or object at a tile
	# For demonstration, we won't do anything here yet
	pass
