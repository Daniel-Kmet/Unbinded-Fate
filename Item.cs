using Godot;

[GlobalClass]
public partial class Item : Resource
{
	[Export] public string name;
	[Export] public int damage;
	[Export] public int level;
	[Export] public string animationName;
	[Export] public Texture2D texture;
}
