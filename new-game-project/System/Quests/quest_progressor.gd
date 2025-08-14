@tool
extends Node

class_name QuestProgressor

@export var quest: Quest
var _step_name
var steps : Array[String]:
	get:
		return steps
	set(value):
		steps = value
		update_configuration_warnings()
		notify_property_list_changed()
var parent

func _ready() -> void:
	parent = get_parent()
	if quest:
		if steps.size() < quest.steps.size():
			for step in quest.steps:
				steps.append(step.description)
	await parent.ready
	match quest.steps[_step_name].step_type:
		QuestSteps.types.KILL:
				(parent as NPC).dying.connect(progress)
		QuestSteps.types.FETCH:
				(parent as Interactable).interacted.connect(progress)
		QuestSteps.types.TRIGGER:
				(get_tree().get_first_node_in_group("Player") as Player).trigger.triggered.connect(progress)
			

func _get_property_list():
	var properties = []
	properties.append({
		"name": "_step_name",
		"type": TYPE_STRING,
		"hint": PROPERTY_HINT_ENUM,
		"hint_string": _array_to_string(steps),
	})
	return properties


#helper functions
func _array_to_string(arr: Array[String], separator = ",") -> String:
	var string = ""
	for i in arr:
		string += str(i) + separator
	return string

func progress() -> void:
	if quest.current_step != _step_name:
		return
	quest.progress_quest()
	PlayerStats.hud.quest_box.display_quest(quest)
	self.queue_free()
