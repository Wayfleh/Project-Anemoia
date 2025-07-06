extends HitBox

var cached_closest : Punchable

func _ready():
	controller = get_tree().get_nodes_in_group("Player")[0]

func _physics_process(delta: float) -> void:
	direction = (controller as Player).camera.global_transform.basis.z
	if monitoring:
		var new_closest : Punchable = get_closest_punchable()
		if new_closest != cached_closest:
			cached_closest = new_closest
		attack

func attack():
	if cached_closest.controller.is_in_group("Enemies"):
		hurt(cached_closest)
