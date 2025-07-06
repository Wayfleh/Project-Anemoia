extends StateMachine

@onready var controller = get_parent()

func _ready():
	await owner.ready
	current_state = $FreeMove
	SignalBus.dialogue_ended.connect(end_dialogue)

func start_dialogue(freeze : bool):
	if freeze:
		current_state = $Frozen

func end_dialogue():
	current_state = $FreeMove
