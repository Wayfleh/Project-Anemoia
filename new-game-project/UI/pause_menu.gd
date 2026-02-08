extends MenuScreen

@onready var resume_button: Button = find_child("Resume")
@onready var restart_button: Button = find_child("Restart")
@onready var quit_button: Button = find_child("Quit")
@onready var save_button: Button = find_child("Save")
@onready var load_button: Button = find_child("Load")

@onready var debug_button: Button = find_child("Debug")

@onready var debug_menu = $"Debug Menu"


func _ready():
	$AnimationPlayer.play("RESET")
	resume_button.pressed.connect(resume)
	restart_button.pressed.connect(restart)
	quit_button.pressed.connect(get_tree().quit)
	debug_button.pressed.connect(debug)
	

func resume():
	get_tree().paused = false
	$AnimationPlayer.play("Unpause")
	#checks if player state is not null or in Frozen dialogue
	if UI.player_state and UI.player_state is not Frozen:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	debug_menu.visible = false
	_resetUIState()

func pause():
	get_tree().paused = true
	$AnimationPlayer.play("Pause")
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	
func restart():
	resume()
	get_tree().reload_current_scene()

func debug():
	debug_menu.visible = true

func _on_save_pressed():
	if !get_tree().paused:
		return
	%SaveLoader.save_game()

func _on_load_pressed():
	if !get_tree().paused:
		return
	%SaveLoader.load_game()
