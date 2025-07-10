extends Punchable
class_name HurtBox

var body : CharacterBody3D = get_parent()
var cooldown : bool = false

func _ready() -> void:
	controller = owner

func cooldown_end():
	cooldown = false
