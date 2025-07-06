extends CharacterBody3D
class_name Player

const WALK_SPEED = 5.0


#fov variables
const BASE_FOV = 75.0
const FOV_CHANGE = 1.5

#step variables
const MAX_STEP_HEIGHT = 0.5
var _snapped_to_stairs_last_frame = false
var _last_frame_was_on_floor = -INF

var gravity = 9.8

@onready var head = $Head
@onready var camera = $Head/CameraSmooth/Camera3D
@onready var dia_state_machine = $DialogueStates
@onready var current_state: StateMachineState = dia_state_machine.current_state
@onready var collider = $PlayerCollider
@onready var trigger = $Trigger
@onready var arm_animator = $ArmAnimations
@onready var arm = %Arm

func _ready():
	arm_animator.animation_finished.connect(_on_animation_finished)
##------------------------------------------------------------------------------------------
## Handling non-physics related inputs
#
#func _unhandled_input(event: InputEvent) -> void:
	#if event is InputEventMouseMotion:
		#if Input.is_action_pressed("Free Camera"):
			#Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		#else:
			#Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
			#head.rotate_y(-event.relative.x * SENSITIVITY)
			#camera.rotate_x(-event.relative.y * SENSITIVITY)
			#camera.rotation.x = clamp(camera.rotation.x, deg_to_rad(-40), deg_to_rad(60))
##------------------------------------------------------------------------------------------

# Animation Handling
func _on_animation_finished(name: String):
	if name == "Punch":
		arm_animator.play("Idle")

func _physics_process(delta: float) -> void:
	current_state = dia_state_machine.current_state
	if is_on_floor() or _snapped_to_stairs_last_frame: _last_frame_was_on_floor = Engine.get_physics_frames()
	
	# Add the gravity.
	if not is_on_floor() or _snapped_to_stairs_last_frame:
		velocity.y -= gravity * delta
#
	if not _snap_up_stairs_check(delta):
		_snap_down_to_stairs_check()
	
	## Handle jump.
	#if Input.is_action_just_pressed("jump") and is_on_floor():
		#velocity.y = JUMP_VELOCITY
	#
	## Handle sprint.
	#if Input.is_action_pressed("sprint") and Input.is_action_pressed("up"):
		#speed = SPRINT_SPEED
	#else:
		#speed = WALK_SPEED
#
	## Get the input direction and handle the movement/deceleration.
	## As good practice, you should replace UI actions with custom gameplay actions.
	#var input_dir = Input.get_vector("left","right","up","down")
	#var direction = (head.transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	#if is_on_floor() or _snapped_to_stairs_last_frame:
		#if direction:
			#velocity.x = direction.x * speed
			#velocity.z = direction.z * speed
		#else:
			#velocity.x = lerp(velocity.x, direction.x * speed, delta * 7)
			#velocity.z = lerp(velocity.z, direction.z * speed, delta * 7)
	#else:
		#velocity.x = lerp(velocity.x, direction.x * speed, delta * 3.0)
		#velocity.z = lerp(velocity.z, direction.z * speed, delta * 3.0)
	#
	## Head Bob
	#t_bob += delta * velocity.length() * float(is_on_floor())
	#camera.transform.origin = _headbob(t_bob)
	## Head Tilt
	#if Input.is_action_pressed("left"):
		#camera.rotation.z = lerp(camera.rotation.z, head_tilt, delta * 4)
	#elif Input.is_action_pressed("right"):
		#camera.rotation.z = lerp(camera.rotation.z, -head_tilt, delta * 4)
	#else:
		#camera.rotation.z = lerp(camera.rotation.z, 0.0, delta * 7)
	#if Input.is_action_pressed("up"):
		#head_tilt = deg_to_rad(1)
	#else:
		#head_tilt = deg_to_rad(2)
	##
	## FOV
	#var velocity_clamped = clamp(velocity.length(), 0.5, SPRINT_SPEED * 2)
	#var target_fov = BASE_FOV;
	#if Input.is_action_pressed("sprint") and Input.is_action_pressed("up"):
		#target_fov = BASE_FOV + FOV_CHANGE * velocity_clamped
	#else:
		#target_fov = BASE_FOV
		#
	#camera.fov = lerp(camera.fov, target_fov, delta * 8)
	
#------------------------------------------------------------------------------------------
# Helper Functions

# passes normal vector of surface. Checks if normal vector angle is greater than
# max angle to consider a surface a floor (i.e. is it too steep)
func is_surface_too_steep(normal : Vector3) -> bool:
	return normal.angle_to(Vector3.UP) > self.floor_max_angle

func _run_body_test_motion(from :Transform3D, motion : Vector3, result = null) -> bool:
	if not result: result = PhysicsTestMotionResult3D.new()
	var params = PhysicsTestMotionParameters3D.new()
	params.from = from
	params.motion = motion
	return PhysicsServer3D.body_test_motion(self.get_rid(), params, result)

#------------------------------------------------------------------------------------------
# Polish

# Steps and Stairs
func _snap_up_stairs_check(delta) -> bool:
	if not is_on_floor() and not _snapped_to_stairs_last_frame: return false
	var expected_move_motion = self.velocity * Vector3(1,0,1) * delta
	var step_pos_with_clearance = self.global_transform.translated(expected_move_motion + Vector3(0,MAX_STEP_HEIGHT * 2, 0))
	# Run a body_test_motion slightly above the pos we expect ot move to, towards the floor.
	# We give some clearance above to ensure there's ample room for the player.
	# If it hits a step <= MAX_STEP_HEIGHT, we can teleport the player on top of the step
	# along with their intended motion forward.
	var down_check_result = PhysicsTestMotionResult3D.new()
	if (_run_body_test_motion(step_pos_with_clearance, Vector3(0,-MAX_STEP_HEIGHT * 2,0), down_check_result) and (down_check_result.get_collider().is_class("StaticBody3D") or down_check_result.get_collider().is_class("CSGShape3D"))):
		var step_height = ((step_pos_with_clearance.origin + down_check_result.get_travel()) - self.global_position).y
		if step_height > MAX_STEP_HEIGHT or step_height <= 0.01 or (down_check_result.get_collision_point() - self.global_position).y > MAX_STEP_HEIGHT: return false
		%StairsAheadRay.global_position = down_check_result.get_collision_point() + Vector3(0,MAX_STEP_HEIGHT,0) + expected_move_motion.normalized() * 0.1
		%StairsAheadRay.force_raycast_update()
		if %StairsAheadRay.is_colliding() and not is_surface_too_steep(%StairsAheadRay.get_collision_normal()):
			_save_camera_pos_for_smoothing()
			self.global_position = step_pos_with_clearance.origin + down_check_result.get_travel()
			apply_floor_snap()
			_snapped_to_stairs_last_frame = true
			return true
	return false

func _snap_down_to_stairs_check() -> void:
	var did_snap := false
	var floor_below : bool = %StairsBelowRay.is_colliding() and not is_surface_too_steep(%StairsBelowRay.get_collision_normal())
	var was_on_floor_last_frame = Engine.get_physics_frames() - _last_frame_was_on_floor == 1
	if not is_on_floor() and velocity.y <= 0 and (was_on_floor_last_frame or _snapped_to_stairs_last_frame) and floor_below:
		var body_test_result = PhysicsTestMotionResult3D.new()
		if _run_body_test_motion(self.global_transform, Vector3(0, -MAX_STEP_HEIGHT, 0), body_test_result):
			_save_camera_pos_for_smoothing()
			var translate_y = body_test_result.get_travel().y
			self.position.y += translate_y
			apply_floor_snap()
			did_snap = true
	_snapped_to_stairs_last_frame = did_snap


var _saved_camera_global_pos = null
func _save_camera_pos_for_smoothing():
	if _saved_camera_global_pos == null:
		_saved_camera_global_pos = %CameraSmooth.global_position

func _slide_camera_smooth_back_to_origin(delta):
	if _saved_camera_global_pos == null: return
	%CameraSmooth.global_position.y = _saved_camera_global_pos.y
	%CameraSmooth.position.y = clampf(%CameraSmooth.position.y, -0.7, 0.7)
	var move_amount = max(self.velocity.length() * delta, WALK_SPEED/2 * delta)
	%Camera.position.y = move_toward(%CameraSmooth.position.y, 0.0, move_amount)
	_saved_camera_global_pos = %CameraSmooth.global_position
	if %CameraSmooth.position.y == 0:
		_saved_camera_global_pos = null

func on_load_game():
	return
