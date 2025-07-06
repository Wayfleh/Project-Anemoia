extends CharacterBody3D

@onready var health = 5
@onready var gravity = 9.8

func _ready():
	$HurtBox.hurt.connect(take_damage)

func take_damage(hitbox : HitBox):
	health -= 1
	print(health)
	
func _physics_process(delta: float) -> void:
	if not is_on_floor():
		velocity.y -= gravity * delta
	
	move_and_slide()
