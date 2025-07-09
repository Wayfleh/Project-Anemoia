extends CharacterBody3D

@onready var health = 5
@onready var gravity = 9.8
@onready var drag = 3

func _ready():
	$HurtBox.hurt.connect(take_damage)

func take_damage(hitbox : HitBox):
	health -= 1
	velocity += -hitbox.direction * 5
	velocity.y += 10
	print(health)
	
func _physics_process(delta: float) -> void:
	if not is_on_floor():
		velocity.y -= gravity * delta
	velocity.x -= velocity.x * delta * drag
	velocity.z -= velocity.z * delta * drag
	
	move_and_slide()
