extends SubViewport


func _ready() -> void:
	UI.window_size = size
	QuestHandler.add_enemy.connect(_add_enemy)

func _add_enemy(enemy : Enemy):
	get_child(0).add_child(enemy)
