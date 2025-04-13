using Godot;

[GlobalClass]
public partial class Item : Resource
{
	[Export] public string name;
	[Export] public float damage;
	[Export] public int level;
	[Export] public string animationName;
	[Export] public Texture2D texture;
	[Export] public string projectileScene;
	[Export] public float fireRate = 0.5f;
}
