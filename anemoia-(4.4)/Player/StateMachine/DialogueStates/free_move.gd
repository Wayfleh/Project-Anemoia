extends DialogueState

const WALK_SPEED = 5.0
const SPRINT_SPEED = 10.0
const JUMP_VELOCITY = 4.5
const SENSITIVITY = 0.003
var speed

#head movement variables
const BOB_FREQ = 2
const BOB_AMP = 0.08
var t_bob = 0.0
var head_tilt = deg_to_rad(3)

var camera: Camera3D
var head: Node3D

func _enter_state():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
#------------------------------------------------------------------------------------------
# Handling non-physics related inputs

func _unhandled_input(event: InputEvent) -> void:
	camera = controller.camera
	if is_current_state():
		if event is InputEventMouseMotion:
			if Input.is_action_pressed("Free Camera"):
				Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			else:
				Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
				controller.head.rotate_y(-event.relative.x * SENSITIVITY)
				camera.rotate_x(-event.relative.y * SENSITIVITY)
				camera.rotation.x = clamp(camera.rotation.x, deg_to_rad(-70), deg_to_rad(60))
#------------------------------------------------------------------------------------------

func _physics_process(delta: float) -> void:
	camera = controller.camera
	head = controller.head
	if is_current_state():

		# Handle jump.
		if Input.is_action_just_pressed("jump") and controller.is_on_floor():
			controller.velocity.y = JUMP_VELOCITY
		
		# Handle sprint.
		if Input.is_action_pressed("sprint") and Input.is_action_pressed("up"):
			speed = SPRINT_SPEED
		else:
			speed = WALK_SPEED
		
		# Handle Attack
		if Input.is_action_just_pressed("Attack"):
			controller.arm_animator.play("Punch")

		# Get the input direction and handle the movement/deceleration.
		var input_dir = Input.get_vector("left","right","up","down")
		var direction = (controller.head.transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
		if controller.is_on_floor() or controller._snapped_to_stairs_last_frame:
			if direction:
				controller.velocity.x = direction.x * speed
				controller.velocity.z = direction.z * speed
			else:
				controller.velocity.x = lerp(controller.velocity.x, direction.x * speed, delta * 7)
				controller.velocity.z = lerp(controller.velocity.z, direction.z * speed, delta * 7)
		else:
			controller.velocity.x = lerp(controller.velocity.x, direction.x * speed, delta * 3.0)
			controller.velocity.z = lerp(controller.velocity.z, direction.z * speed, delta * 3.0)
		
		# Head Bob
		t_bob += delta * controller.velocity.length() * float(controller.is_on_floor())
		camera.transform.origin = _headbob(t_bob)
		# FOV
		var velocity_clamped = clamp(controller.velocity.length(), 0.5, SPRINT_SPEED * 2)
		var target_fov = controller.BASE_FOV;
		
		if Input.is_action_pressed("sprint") and Input.is_action_pressed("up"):
			target_fov = controller.BASE_FOV + controller.FOV_CHANGE * velocity_clamped
		else:
			target_fov = controller.BASE_FOV
			
		camera.fov = lerp(camera.fov, target_fov, delta * 8)
			
		if Input.is_action_pressed("left"):
			head.rotation.z = lerp(head.rotation.z, head_tilt, delta * 4)
		elif Input.is_action_pressed("right"):
			head.rotation.z = lerp(head.rotation.z, -head_tilt, delta * 4)
		else:
			head.rotation.z = lerp(head.rotation.z, 0.0, delta * 7)
		if Input.is_action_pressed("up"):
			head_tilt = deg_to_rad(1)
		else:
			head_tilt = deg_to_rad(2)
		
		
		if not controller._snap_up_stairs_check(delta):
			controller.move_and_slide()


# Camera Movement
func _headbob(time) -> Vector3:
	var pos = Vector3.ZERO
	pos.y = sin(time * BOB_FREQ) * BOB_AMP
	pos.x = cos(time * BOB_FREQ * .5) * BOB_AMP * 2
	return pos
