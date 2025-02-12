extends Node2D

@export var village_chunk_scene: PackedScene
@export var random_chunk_scenes: Array[PackedScene] = []

@export var chunk_size: Vector2 = Vector2(256, 256)
@export var chunk_view_range: int = 2

@export var player_path: NodePath
var player: Node2D

@export var world_seed: int = 12345

# Tracks spawned chunks so we don't duplicate them
var spawned_chunks = {}

func _ready() -> void:
	print(">>> WorldGenerator _ready() called.")
	if player_path:
		player = get_node_or_null(player_path) as Node2D
		if player:
			print(">>> Found player node:", player)
		else:
			print(">>> WARNING: Could not find Player at:", player_path)
	else:
		print(">>> WARNING: player_path is not set.")

	# Always load the village chunk at (0,0)
	print(">>> Spawning village chunk at origin (0,0).")
	spawn_chunk(Vector2i(0, 0))

func _physics_process(_delta: float) -> void:
	if not player:
		return

	# Check the player's chunk coordinate
	var player_chunk_coord = world_to_chunk_coord(player.global_position)
	print(">>> _physics_process() - Player chunk coord:", player_chunk_coord)

	# Spawn new chunks around the player if they're not already spawned
	for x in range(player_chunk_coord.x - chunk_view_range, player_chunk_coord.x + chunk_view_range + 1):
		for y in range(player_chunk_coord.y - chunk_view_range, player_chunk_coord.y + chunk_view_range + 1):
			spawn_chunk(Vector2i(x, y))

	# NOTE: We do NOT unload any chunks. 
	# Once spawned, they remain in the scene permanently.

func spawn_chunk(chunk_coord: Vector2i) -> void:
	print(">>> spawn_chunk() called for:", chunk_coord)

	if spawned_chunks.has(chunk_coord):
		print(">>> Chunk already spawned at:", chunk_coord, "- Skipping.")
		return

	var chunk_scene: PackedScene

	if chunk_coord == Vector2i(0, 0):
		print(">>> Using village_chunk_scene for origin.")
		chunk_scene = village_chunk_scene
	else:
		# Determine the random chunk (Forest, Desert, etc.) for this coordinate
		chunk_scene = get_chunk_scene_for_coord(chunk_coord)

	print(">>> Instantiating scene:", chunk_scene, "at coord:", chunk_coord)
	var new_chunk = chunk_scene.instantiate() as Node2D
	new_chunk.position = chunk_coord_to_world_position(chunk_coord)
	add_child(new_chunk)
	spawned_chunks[chunk_coord] = new_chunk

	print(">>> Chunk spawned successfully:", chunk_coord)

func get_chunk_scene_for_coord(chunk_coord: Vector2i) -> PackedScene:
	if random_chunk_scenes.size() == 0:
		push_warning("No random_chunk_scenes assigned! Using village scene as fallback.")
		return village_chunk_scene

	# Create a local RNG using a deterministic seed so the same coord always yields the same scene
	var rng = RandomNumberGenerator.new()
	rng.seed = get_chunk_seed(chunk_coord)

	var index = rng.randi() % random_chunk_scenes.size()
	var chosen_scene = random_chunk_scenes[index]

	print(">>> get_chunk_scene_for_coord(): coord =", chunk_coord, ", seed =", rng.seed,
		  ", chosen index =", index, ", scene =", chosen_scene)

	return chosen_scene

func get_chunk_seed(chunk_coord: Vector2i) -> int:
	var x_val = chunk_coord.x * 374761393
	var y_val = chunk_coord.y * 668265263
	var seed_result = x_val + y_val + world_seed

	# Debug print for the seed formula
	print(">>> get_chunk_seed(): coord =", chunk_coord, ", world_seed =", world_seed, ", result =", seed_result)

	return seed_result

func world_to_chunk_coord(world_pos: Vector2) -> Vector2i:
	var cx = int(floor(world_pos.x / chunk_size.x))
	var cy = int(floor(world_pos.y / chunk_size.y))
	return Vector2i(cx, cy)

func chunk_coord_to_world_position(chunk_coord: Vector2i) -> Vector2:
	return Vector2(
		float(chunk_coord.x) * chunk_size.x,
		float(chunk_coord.y) * chunk_size.y
	)
