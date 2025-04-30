using Godot;
using System;
using System.Collections.Generic;

public partial class WorldGenerator : Node2D
{
	[Export] public PackedScene VillageChunkScene { get; set; }
	[Export] public Godot.Collections.Array<PackedScene> RandomChunkScenes { get; set; } = new();
	
	[Export] public Vector2 ChunkSize { get; set; } = new Vector2(256, 256);
	[Export] public int ChunkViewRange { get; set; } = 2;
	[Export] public int MaxLoadedChunks { get; set; } = 100; // Limit total loaded chunks to prevent memory issues
	[Export] public NodePath PlayerPath { get; set; }
	[Export] public int WorldSeed { get; set; } = 12345;
	
	private Node2D _player;
	private readonly Dictionary<Vector2I, Node2D> _spawnedChunks = new();
	private readonly RandomNumberGenerator _rng = new();
	private Vector2I _lastPlayerChunkCoord;
	
	public override void _Ready()
	{
		if (!PlayerPath.IsEmpty)
		{
			_player = GetNode<Node2D>(PlayerPath);
			if (_player != null)
			{
				// Ensure player is always on top
				_player.ZIndex = 10;
			}
		}
		
		_rng.Seed = (ulong)WorldSeed;
		
		// Always spawn the village chunk at (0,0)
		SpawnChunk(new Vector2I(0, 0));
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_player == null) return;
		
		// Get player's current chunk coordinate
		var playerChunkCoord = WorldToChunkCoord(_player.GlobalPosition);
		
		// Only update chunks if player has moved to a new chunk or this is the first update
		if (playerChunkCoord != _lastPlayerChunkCoord || _lastPlayerChunkCoord == Vector2I.Zero)
		{
		// Spawn chunks around the player
		for (int x = playerChunkCoord.X - ChunkViewRange; x <= playerChunkCoord.X + ChunkViewRange; x++)
		{
			for (int y = playerChunkCoord.Y - ChunkViewRange; y <= playerChunkCoord.Y + ChunkViewRange; y++)
			{
				SpawnChunk(new Vector2I(x, y));
			}
			}
			
			// Unload chunks that are too far from the player
			UnloadDistantChunks(playerChunkCoord);
			
			_lastPlayerChunkCoord = playerChunkCoord;
		}
	}
	
	private void SpawnChunk(Vector2I chunkCoord)
	{
		if (_spawnedChunks.ContainsKey(chunkCoord)) return;
		
		PackedScene chunkScene;
		
		if (chunkCoord == new Vector2I(0, 0))
		{
			chunkScene = VillageChunkScene;
		}
		else
		{
			chunkScene = GetChunkSceneForCoord(chunkCoord);
		}
		
		if (chunkScene == null)
		{
			return;
		}
		
		var newChunk = chunkScene.Instantiate<Node2D>();
		newChunk.Position = ChunkCoordToWorldPosition(chunkCoord);
		
		// Set Z-index based on chunk type
		if (chunkCoord == new Vector2I(0, 0))
		{
			// Village chunk is always on top
			newChunk.ZIndex = 1;
		}
		else
		{
			// Other chunks are below
			newChunk.ZIndex = 0;
		}
		
		AddChild(newChunk);
		_spawnedChunks[chunkCoord] = newChunk;
	}
	
	private void UnloadDistantChunks(Vector2I playerChunkCoord)
	{
		// Don't unload if we're under the chunk limit
		if (_spawnedChunks.Count <= MaxLoadedChunks) return;
		
		// Calculate unload range (chunks outside this range will be unloaded)
		int unloadRange = ChunkViewRange + 2;
		
		// Create a list of chunks to unload
		List<Vector2I> chunksToUnload = new List<Vector2I>();
		
		foreach (var chunkEntry in _spawnedChunks)
		{
			Vector2I coord = chunkEntry.Key;
			
			// Never unload the village chunk
			if (coord == Vector2I.Zero) continue;
			
			// Calculate Manhattan distance to determine if chunk should be unloaded
			int distance = Mathf.Abs(coord.X - playerChunkCoord.X) + Mathf.Abs(coord.Y - playerChunkCoord.Y);
			
			if (distance > unloadRange)
			{
				chunksToUnload.Add(coord);
			}
		}
		
		// Unload the chunks
		foreach (var coord in chunksToUnload)
		{
			if (_spawnedChunks.TryGetValue(coord, out Node2D chunk) && chunk != null)
			{
				chunk.QueueFree();
				_spawnedChunks.Remove(coord);
			}
		}
	}
	
	private PackedScene GetChunkSceneForCoord(Vector2I chunkCoord)
	{
		if (RandomChunkScenes.Count == 0)
		{
			return VillageChunkScene;
		}
		
		// Create deterministic seed for this coordinate
		var chunkSeed = GetChunkSeed(chunkCoord);
		_rng.Seed = unchecked((ulong)chunkSeed);
		
		var index = (int)(_rng.Randi() % RandomChunkScenes.Count);
		return RandomChunkScenes[index];
	}
	
	private int GetChunkSeed(Vector2I chunkCoord)
	{
		// Use bitwise operations to avoid overflow
		int hash = 17;
		hash = hash * 31 + chunkCoord.X;
		hash = hash * 31 + chunkCoord.Y;
		hash = hash * 31 + WorldSeed;
		return hash;
	}
	
	private Vector2I WorldToChunkCoord(Vector2 worldPos)
	{
		var cx = (int)Mathf.Floor(worldPos.X / ChunkSize.X);
		var cy = (int)Mathf.Floor(worldPos.Y / ChunkSize.Y);
		return new Vector2I(cx, cy);
	}
	
	private Vector2 ChunkCoordToWorldPosition(Vector2I chunkCoord)
	{
		return new Vector2(
			chunkCoord.X * ChunkSize.X,
			chunkCoord.Y * ChunkSize.Y
		);
	}
} 
