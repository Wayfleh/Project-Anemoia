extends Node

#This grabs the ToBeSaved group
@onready var ToBeSaved = get_tree().get_nodes_in_group("ToBeSaved")
#This one will grab the current map and grab the player from the map
@onready var player = %SubViewport.get_child(0).get_node("%Player")

func save_game():
	var saved_data: SavedData = SavedData.new()
	
	saved_data.player_position = player.global_position
	saved_data.player_stat_list = PlayerStats.stat_list
	saved_data.personality_list = PlayerStats.personality_list
	
	get_tree().call_group("Interactables", "on_save_game", saved_data)
	ResourceSaver.save(saved_data, SavedData.SAVE_GAME_PATH)

func load_game():
	var saved_data:SavedData = load(SavedData.SAVE_GAME_PATH)
	player.global_position = saved_data.player_position
	PlayerStats.stat_list = saved_data.player_stat_list
	PlayerStats.personality_list = saved_data.personality_list
	
	get_tree().call_group("Interactables", "on_load_game", saved_data)
	get_tree().call_group("ToBeSaved", "on_load_game")
