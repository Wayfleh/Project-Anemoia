extends Punchable

var body : CharacterBody3D = get_parent()

func _ready() -> void:
	controller = owner
