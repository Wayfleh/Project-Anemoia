extends Control

class_name HUD

@onready var quest_box : QuestDisplay = $Quest

func _ready() -> void:
	QuestHandler.hud = self

func on_load_game():
	quest_box.display_quest(QuestHandler.current_quest)
