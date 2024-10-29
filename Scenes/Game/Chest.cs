using Godot;
using System;
using System.Collections.Generic;

public partial class Chest : Area2D
{
    AnimationPlayer chestAnim;
    Shop chestShop;
    PackedScene shopUI;
    public bool isOpened = false;
    public bool isShopClosed = false;
    public bool isPlayerNearChest = false;
    public override void _Ready()
	{
		chestAnim = GetNode<AnimationPlayer>("Chest Interaction");	
	}
    public override void _Process(double delta)
    {
        if (isPlayerNearChest == true)
        {
            if (Input.IsActionJustPressed("interact") && isOpened == false)
            {
                OpenChest();
            }
        }
    }

    public void OpenChest()
    {
        chestAnim.Play("Open Chest");
        isOpened = true;
        shopUI = ResourceLoader.Load<PackedScene>("res://Scenes/Shop/Shop.tscn");
		chestShop = (Shop)shopUI.Instantiate();
		GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/World").AddChild(chestShop);
	}

    private void _onChestAreaEntered(Player player)
	{
		if (player.Name == "Player")
		{
			isPlayerNearChest = true;
		}
	}
    private void _onChestAreaExited(Player player)
	{
		if (player.Name == "Player")
		{
			isPlayerNearChest = false;
		}
	}
}
