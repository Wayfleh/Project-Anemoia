extends Area3D

func _ready():
	area_entered.connect(on_area_entered)

func on_area_entered(area : Area3D):
	if area.is_in_group("TalkingTriggers"):
		if !area.already_talked:
			get_parent().dia_state_machine.start_dialogue(area.freeze_player)
			area.start_dialogue()
