class_name Enemy
extends CharacterBody3D

var health = 5
@onready var gravity = 9.8
@onready var drag = 3
var MOVE_SPEED = 7
var STUN_TIMER = .2
var dead = false
@onready var hurt_box = $HurtBox
var player: Player

signal dying

func _ready():
	$HurtBox.hurt.connect(take_damage)
	player = get_tree().get_nodes_in_group("Player")[0]

func take_damage(hitbox : HitBox):
	health -= 1
	$HurtBox.cooldown = true
	if health <= 0:
		velocity += -hitbox.direction * 15
		velocity.y += 5
		var timer = get_tree().create_timer(1)
		timer.timeout.connect(die)
		return
	var timer = get_tree().create_timer(STUN_TIMER)
	velocity += -hitbox.direction * 5
	velocity.y += 2
	timer.timeout.connect(hurtbox_timeout)

func die():
	dead = true
	dying.emit()
	queue_free()

func hurtbox_timeout():
	$HurtBox.cooldown_end()

func _physics_process(delta: float) -> void:
	if not is_on_floor():
		velocity.y -= gravity * delta
		drag = 0
	else:
		drag = 3
	
	var direction = (transform.basis * Vector3(player.global_position.x - global_position.x, 0, player.global_position.z - global_position.z)).normalized()
	if is_on_floor():
		if direction && !$HurtBox.cooldown:
			velocity.x = direction.x * MOVE_SPEED
			velocity.z = direction.z * MOVE_SPEED
		else:
			velocity.x -= velocity.x * delta * drag
			velocity.z -= velocity.z * delta * drag
	move_and_slide()
