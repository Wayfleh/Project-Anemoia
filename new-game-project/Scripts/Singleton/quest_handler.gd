extends Node

@onready var quest_log : Array[Quest] = []
var enemy_scene = preload("res://System/Combat/Enemies/Combat_test_dummy.tscn")
var quest_progressor_scene = preload("res://System/Quests/quest_progressor.tscn")
var current_quest : Quest
var hud : HUD

func _ready() -> void:
	hud = get_tree().get_first_node_in_group("HUD")

func add_quest(quest : Quest) -> void:
	quest_log.append(quest)
	quest.quest_completed.connect(_quest_end)
	quest.step_completed.connect(_step_handler)

#Updates step based on step type
func _step_handler(quest : Quest) -> void:
	var curr_step : QuestSteps = quest.steps[quest.current_step]
	match curr_step.step_type:
		QuestSteps.types.KILL:
			_spawn_enemy(quest, quest.current_step)
			return
		QuestSteps.types.FETCH:
			return
		QuestSteps.types.TALK:
			return
		QuestSteps.types.TRIGGER:
			return

func _quest_end(quest : Quest) -> void:
	return

func _create_enemy_for_quest(quest: Quest, step: int) -> Enemy:
	var enemy: Enemy = enemy_scene.instantiate()
	var quest_p: QuestProgressor = quest_progressor_scene.instantiate()
	enemy.add_child(quest_p)
	quest_p.quest = quest
	quest_p.step_name = quest.steps[step].description
	
	return enemy

func _spawn_enemy(quest: Quest, step: int) -> void:
	var quer_step: QuestSteps = quest.steps[step]
	for i in range(quer_step.condition):
		print("hello child has been added")
		var enemy := _create_enemy_for_quest(quest, step)
		enemy.global_position.y += 5
		UI.world.add_child(enemy)
		await get_tree().create_timer(1).timeout
