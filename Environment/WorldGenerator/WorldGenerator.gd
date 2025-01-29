extends Node2D

@export var village_chunk_scene: PackedScene
@export var random_chunk_scenes: Array[PackedScene] = []
@export var chunk_size: Vector2 = Vector2(256, 256)
@export var chunk_view_range: int = 2

@export var player_path: NodePath
var player: Node2D

# Keep track of which chunk instance is currently spawned at each coordinate
var spawned_chunks: Dictionary[Vector2i, Node2D] = {}

# NEW: Keep track of which chunk scene was assigned to each coordinate
var assigned_chunks: Dictionary[Vector2i, PackedScene] = {}

func _ready() -> void:
	if player_path:
		player = get_node_or_null(player_path) as Node2D
	spawn_chunk(Vector2i(0, 0))  # Always spawn the village at origin

func _physics_process(_delta: float) -> void:
	if not player:
		return

	var player_chunk_coord = world_to_chunk_coord(player.global_position)

	# Spawn any missing chunks in view range
	for x in range(player_chunk_coord.x - chunk_view_range, player_chunk_coord.x + chunk_view_range + 1):
		for y in range(player_chunk_coord.y - chunk_view_range, player_chunk_coord.y + chunk_view_range + 1):
			spawn_chunk(Vector2i(x, y))

	# Unload chunks outside view range (but not the village at (0,0))
	for coord in spawned_chunks.keys():
		if coord == Vector2i(0, 0):
			continue
		if abs(coord.x - player_chunk_coord.x) > chunk_view_range or abs(coord.y - player_chunk_coord.y) > chunk_view_range:
			unload_chunk(coord)

func spawn_chunk(chunk_coord: Vector2i) -> void:
	if spawned_chunks.has(chunk_coord):
		# Already loaded and spawned in the scene
		return

	# 1. Determine which chunk scene to use for this coordinate
	var chunk_scene: PackedScene
	if chunk_coord == Vector2i(0, 0):
		# Always the village at origin
		chunk_scene = village_chunk_scene
	else:
		# If we've never assigned a chunk to this coordinate, pick a random one
		if not assigned_chunks.has(chunk_coord):
			if random_chunk_scenes.size() == 0:
				push_warning("No random_chunk_scenes defined!")
				return
			var index = randi() % random_chunk_scenes.size()
			assigned_chunks[chunk_coord] = random_chunk_scenes[index]
		# Re-use the scene that was assigned before
		chunk_scene = assigned_chunks[chunk_coord]

	# 2. Instantiate the chunk
	var chunk_instance = chunk_scene.instantiate() as Node2D
	chunk_instance.position = chunk_coord_to_world_position(chunk_coord)
	add_child(chunk_instance)

	# 3. Record that this coordinate is currently spawned
	spawned_chunks[chunk_coord] = chunk_instance

func unload_chunk(chunk_coord: Vector2i) -> void:
	if spawned_chunks.has(chunk_coord):
		spawned_chunks[chunk_coord].queue_free()
		spawned_chunks.erase(chunk_coord)

func world_to_chunk_coord(world_pos: Vector2) -> Vector2i:
	var cx = int(floor(world_pos.x / chunk_size.x))
	var cy = int(floor(world_pos.y / chunk_size.y))
	return Vector2i(cx, cy)

func chunk_coord_to_world_position(chunk_coord: Vector2i) -> Vector2:
	return Vector2(
		chunk_coord.x * chunk_size.x,
		chunk_coord.y * chunk_size.y
	)
