extends Interactor

@export var player: CharacterBody3D

var cached_closest: Interactable

func _ready() -> void:
	controller = player

func _physics_process(delta: float) -> void:
	var new_closest: Interactable = get_closest_interactable()
	if new_closest != cached_closest:
		if is_instance_valid(cached_closest):
			unfocus(cached_closest)
		if new_closest:
			focus(new_closest)
		cached_closest = new_closest

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("interact"):
		if cached_closest:
			interact(cached_closest)
			if cached_closest is Talkable:
				SignalBus.start_dialogue(cached_closest.dialogue, cached_closest.talk_back)

func _on_area_exited(area: Interactable) -> void:
	if cached_closest == area:
		unfocus(area)
