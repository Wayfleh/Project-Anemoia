extends Control

@onready var resume_button: Button = find_child("Resume")
@onready var restart_button: Button = find_child("Restart")
@onready var quit_button: Button = find_child("Quit")

func _ready():
	$AnimationPlayer.play("RESET")
	resume_button.pressed.connect(resume)
	restart_button.pressed.connect(restart)
	quit_button.pressed.connect(get_tree().quit)

func resume():
	get_tree().paused = false
	$AnimationPlayer.play("Unpause")
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func pause():
	get_tree().paused = true
	$AnimationPlayer.play("Pause")
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	
func restart():
	resume()
	get_tree().reload_current_scene()
