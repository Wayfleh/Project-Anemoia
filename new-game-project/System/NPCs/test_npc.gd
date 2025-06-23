extends CharacterBody3D

@onready var navigation_agent_3d: NavigationAgent3D = $NavigationAgent3D
var gravity = 9.8
@onready var meshinstance: MeshInstance3D = $MeshInstance3D
@onready var material = StandardMaterial3D.new()
@onready var interacted_with = false
@export var dialogue : JSON
@export var talk_back = true

func _ready() -> void:
	$NPCstates.set_current_state($NPCstates/Idle)
	_move_npc()
	navigation_agent_3d.target_reached.connect(_move_npc)
	%Talkable.focused.connect(_focused)
	%Talkable.unfocused.connect(_unfocused)
	%Talkable.interacted.connect(_interacted)

func _stop_and_talk(interactor: Interactor):
	var player_tran = interactor.controller.global_transform.origin
	meshinstance.look_at(player_tran, Vector3.UP)

func _focused(interactor: Interactor) -> void:
	if not interacted_with:
		material.albedo_color = Color(225, 0, 0, 225)
		meshinstance.set_surface_override_material(0, material)
		print("I'm focused!")

func _unfocused(interactor: Interactor) -> void:
	if not interacted_with:
		material.albedo_color = Color(225,225,225,225)
		meshinstance.set_surface_override_material(0, material)

func _interacted(interactor: Interactor) -> void:
	if not interacted_with:
		material.albedo_color = Color(0, 0, 0, 225)
		meshinstance.set_surface_override_material(0, material)
		interacted_with = true
		$NPCstates.set_current_state($NPCstates/Dialogue)
		_stop_and_talk(interactor)

func _move_npc() -> void:
	var random_position := Vector3.ZERO
	random_position.x = randf_range(-15.0, 15.0)
	random_position.y = randf_range(0.0, 5.0)
	random_position.z = randf_range(-15.0, 15.0)
	navigation_agent_3d.set_target_position(random_position)

func _physics_process(delta: float) -> void:
	var destination = navigation_agent_3d.get_next_path_position()
	var local_destination = destination - global_position
	var direction = local_destination.normalized()
	
	if velocity.x < .5 && velocity.z < .5:
		_move_npc()
	
	if not is_on_floor():
		velocity.y -= gravity * delta
	
	
	velocity = direction * 5.0
	if $NPCstates.current_state is Idle:
		meshinstance.rotation.y = lerp_angle(meshinstance.rotation.y, atan2(-direction.x, -direction.z), 5 * delta)
		move_and_slide()
