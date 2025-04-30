extends SceneTree

func _init():
    print("Debug script started")
    
    # Try to load the ItemShopCollection script
    var script = load("res://Scenes/Shop/ItemShopCollection.gd")
    if script:
        print("Successfully loaded ItemShopCollection.gd")
        
        # Create an instance of the script
        var item_collection = Node.new()
        item_collection.set_script(script)
        
        # Call the get_items function and print the results
        var items = item_collection.get_items()
        print("Items from ItemShopCollection: ", items)
        
        # Check individual items
        if items.size() > 0:
            for i in range(items.size()):
                var item = items[i]
                print("Item ", i, ": ", item)
                print("  Name: ", item.get("name", "No name"))
                print("  Price: ", item.get("price", "No price"))
                print("  Sprite path: ", item.get("sprite_path", "No sprite path"))
                
                # Try to load the texture
                var texture = load(item.get("sprite_path", ""))
                if texture:
                    print("  Texture loaded successfully")
                else:
                    print("  Failed to load texture from path:", item.get("sprite_path", ""))
    else:
        print("Failed to load ItemShopCollection.gd")
    
    print("Debug script finished")
    quit() 