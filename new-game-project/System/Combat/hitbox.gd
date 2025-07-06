extends Area3D
class_name HitBox

var list : Array[Punchable]
@onready var controller
var direction

func hurt(punchable : Punchable):
	punchable.hurt.emit(self)

func get_closest_punchable() -> Punchable:
	var list: Array[Area3D] = get_overlapping_areas()
	var distance: float
	var closest_distance: float = INF
	var closest: Punchable = null
	
	for interactable in list:
		distance = interactable.global_position.distance_to(global_position)
		
		#Sets the first interactable in the list as closest
		if distance < closest_distance:
			closest = interactable as Punchable
			closest_distance = distance
	
	return closest
