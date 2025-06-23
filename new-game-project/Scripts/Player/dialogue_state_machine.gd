extends StateMachine

@onready var controller = get_parent()

func _ready():
	current_state = $FreeMove
	SignalBus.dialogue_initiate.connect(initiate_dialogue)
	SignalBus.dialogue_ended.connect(end_dialogue)

func initiate_dialogue(filename: JSON, talk_back: bool):
	current_state = $Frozen

func end_dialogue():
	current_state = $FreeMove
