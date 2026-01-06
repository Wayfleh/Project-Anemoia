extends Control

enum {GAME, PAUSE, CHARACTER, PERSONALITY}
var current_state: int

var world

func _ready():
	process_mode = Node.PROCESS_MODE_PAUSABLE
	current_state = GAME
	world = get_tree().get_first_node_in_group("World")

func _input(event):
	if event.is_action_pressed("ui_cancel") and get_tree().paused == false and current_state != PAUSE: # Pause Menu
		$PauseMenu.pause()
		current_state = PAUSE
	if event.is_action_pressed("char_menu_button") and get_tree().paused == false and current_state != CHARACTER: # Character Menu
		$CharacterMenu.pause()
		current_state = CHARACTER
	if event.is_action_pressed("pers_menu_button") and get_tree().paused == false and current_state != PERSONALITY:
		$PersonalityMenu.pause()
		current_state = PERSONALITY
