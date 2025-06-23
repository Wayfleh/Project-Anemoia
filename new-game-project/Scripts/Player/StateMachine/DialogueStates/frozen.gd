extends DialogueState

func _enter_state():
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	controller.velocity = Vector3.ZERO

func _unhandled_input(event: InputEvent) -> void:
	if is_current_state():
		pass

func _physics_process(delta: float) -> void:
	if is_current_state():
		pass
