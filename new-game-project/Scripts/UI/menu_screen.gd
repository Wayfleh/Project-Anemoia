extends Control
class_name MenuScreen

@onready var screen: UI = get_parent()

func _resetUIState():
	screen.current_state = UI.GAME
