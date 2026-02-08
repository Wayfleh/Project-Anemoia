extends SubViewport


func _ready() -> void:
	UI.window_size = size
	QuestHandler.add_enemy.connect(_add_enemy)

# Removes the current map from the world and adds a new one (DOESN'T SAVE IT)
func change_the_world(map: String) -> void:
	var current_map = get_child(0)
	remove_child(current_map)
	current_map.queue_free()
	get_tree().get_nodes_in_group("Player").clear() # Clears the global player group so the new scene doesn't grab the old player
	get_tree().get_nodes_in_group("ToBeSaved").clear() # Same for this one
	
	var new_map = load("res://Maps/" + map).instantiate()
	add_child(new_map)
	

func _add_enemy(enemy : Enemy):
	get_child(0).add_child(enemy)
