using Godot;
using System;
using System.Collections;

public partial class Shop : CanvasLayer
{
	Chest chest;
	public AudioStreamPlayer shopMusic;
	AnimatedSprite2D shopCoin;
	AudioStreamPlayer battlemusic;
	
	public override void _Ready()
	{
		shopCoin = GetNode<Control>("Control").GetNode<AnimatedSprite2D>("CoinUI");
		shopMusic = GetNode<AudioStreamPlayer>("Shop Music");
		battlemusic = GetTree().Root.GetNode<AudioStreamPlayer>("SceneManager/CurrentScene/MainGame/Music");
		
		chest = GetTree().Root.GetNode<Chest>("SceneManager/CurrentScene/MainGame/World/Chest");
		shopCoin.Play();
		shopMusic.Play();
		battlemusic.Stop();
	}
	public void _onButtonPressed()
	{
		chest.isShopClosed = true;
		QueueFree();
	}
}
