extends Control
class_name MenuScreen

@onready var screen: UI = get_parent()

func _ready() -> void:
	set_anchors_preset(Control.PRESET_CENTER)

func _resetUIState():
	screen.current_state = UI.GAME
