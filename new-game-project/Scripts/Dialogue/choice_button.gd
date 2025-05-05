class_name ChoiceButton extends Button

var choice_index : int
var continue_button := false

signal choice_selected(choice_index)

func _ready():
	pass

func _on_pressed() -> void:
	choice_selected.emit(choice_index)
