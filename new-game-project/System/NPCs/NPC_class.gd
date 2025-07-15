extends Node
class_name NPC

var gravity: float
var drag: float

var MOVE_SPEED: float
var STUN_TIMER: float

var dead: bool = false

@onready var hurt_box: HurtBox
@onready var player: Player = get_tree().get_nodes_in_group("Player")[0]


func die() -> void:
	dead = true
	queue_free()

func hurtbox_timeout() -> void:
	hurt_box.cooldown_end()
