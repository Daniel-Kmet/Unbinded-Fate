# ItemShopCollection.gd
#Collection of items meant for the shop
extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass




var items = [
	{"name": "Baseball Bat", "price": 10, "sprite_path": "res://Sprites/Weapons/Baseball Bat.png"},
	{"name": "Ruler", "price": 15, "sprite_path": "res://Sprites/Weapons/Ruler Weapon.png"},
	{"name": "Cross", "price": 20, "sprite_path": "res://Sprites/Weapons/Cross Weapon.png"},
	{"name": "Pen", "price": 30, "sprite_path": "res://Sprites/Weapons/Pen Weapon.png"}
]

func get_random_weapon():
	return items[randi() % items.size()]

func get_items():
	print("Items being accessed:", items)
	return items
