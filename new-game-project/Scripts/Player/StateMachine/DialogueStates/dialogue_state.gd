extends StateMachineState
class_name DialogueState

var controller : Player

func _ready() -> void:
	await owner.ready
	controller = owner as Player
	assert(controller != null, "The DialogueState type must be used only in the Player scene. The owner must be a player node")
