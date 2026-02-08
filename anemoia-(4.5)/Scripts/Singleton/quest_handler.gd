extends Node

@onready var quest_log : Array[Quest] = []
@onready var completed_log : Array[Quest] = []
var enemy_scene = preload("res://System/Combat/Enemies/Combat_test_dummy.tscn")
var quest_progressor_scene = preload("res://System/Quests/quest_progressor.tscn")
var current_quest : Quest
var hud : HUD

signal quest_added
signal quest_complete

signal add_enemy(e : Enemy)

func _ready() -> void:
	hud = get_tree().get_first_node_in_group("HUD")

func add_quest(quest : Quest) -> void:
	if quest_log.is_empty():
		current_quest = quest
	quest_log.append(quest)
	quest_added.emit()

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

#when the quest ends, add to completed log and remove from quest log
func _quest_end(quest : Quest) -> void:
	completed_log.append(quest)
	quest_log.erase(quest)
	#the current quest is changed to the top of the quest log
	#current quest is null if quest log is empty
	current_quest = null if quest_log.is_empty() else quest_log[0]
	quest_complete.emit()

func progress_quest(quest : Quest) -> void:
	var _current_step : int = quest.current_step
	quest.progress_quest()
	if quest == current_quest:
		hud.quest_box.display_quest(quest)
	if _current_step != quest.current_step:
		_step_handler(quest)
	if quest.completed:
		_quest_end(quest)

#creates a single enemy for the quest
#TODO change this shit to pass a Callable so different enemy types can be spawned
func _create_enemy_for_quest(quest: Quest, step: int) -> Enemy:
	var enemy: Enemy = enemy_scene.instantiate()
	var quest_p: QuestProgressor = quest_progressor_scene.instantiate()
	enemy.add_child(quest_p)
	quest_p.quest = quest
	quest_p.step_name = quest.steps[step].description
	
	return enemy

#spawns enemy based on the condition of the step
func _spawn_enemy(quest: Quest, step: int) -> void:
	var quer_step: QuestSteps = quest.steps[step]
	for i in range(quer_step.condition):
		print("hello child has been added")
		var enemy := _create_enemy_for_quest(quest, step)
		enemy.global_position.y += 5
		add_enemy.emit(enemy)
		await get_tree().create_timer(1).timeout
