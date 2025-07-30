extends Node

class_name QuestProgressor

@export var quest: Quest
@export var step: int
enum types {KILL, FETCH, TRIGGER, TALK}
@export var step_type: types
@onready var parent

func _ready() -> void:
	parent = get_parent()
	match step_type:
		types.KILL:
				(parent as NPC).dying.connect(progress)
		types.FETCH:
				(parent as Interactable).interacted.connect(progress)
		types.TRIGGER:
				(get_tree().get_first_node_in_group("Player") as Player).trigger.triggered.connect(progress)
			

func progress() -> void:
	if quest.current_step != step:
		return
	quest.progress_quest()
	PlayerStats.hud.quest_box.display_quest(quest)
	self.queue_free()
