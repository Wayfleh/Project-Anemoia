extends HitBox

#var cached_closest : Punchable

func _ready():
	controller = get_tree().get_nodes_in_group("Player")[0]
	area_entered.connect(attack)

func _physics_process(delta: float) -> void:
	direction = (controller as Player).camera.global_transform.basis.z
	#if monitoring == true && has_overlapping_areas():
		#var new_closest : Punchable = get_closest_punchable()
		#if new_closest != cached_closest:
			#cached_closest = new_closest
		#attack()

func attack(cache):
	if cache == null || cache is not Punchable:
		return
	if cache.controller.is_in_group("Enemies") && !cache.cooldown:
		hurt(cache)
