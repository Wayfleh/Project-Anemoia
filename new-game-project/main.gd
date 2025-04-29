extends Control


func _ready():
	process_mode = Node.PROCESS_MODE_PAUSABLE

func _input(event):
	if event.is_action_pressed("ui_cancel") and get_tree().paused == false: # Pause Menu
		$PauseMenu.pause()
	if event.is_action_pressed("char_menu_button") and get_tree().paused == false: # Character Menu
		$CharacterMenu.pause()
