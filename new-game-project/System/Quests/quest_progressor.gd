@tool
extends Node

class_name QuestProgressor

@export var quest: Quest
var step_name: String
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
	await parent.ready
	if quest:
		if steps.size() < quest.steps.size():
			for step in quest.steps:
				steps.append(step.description)
		
	match quest.steps[steps.find(step_name)].step_type:
		QuestSteps.types.KILL:
				(parent as Enemy).dying.connect(progress)
		QuestSteps.types.FETCH:
				(parent as Interactable).interacted.connect(progress.unbind(1))
		QuestSteps.types.TRIGGER:
				(parent as DialogueTrigger).player_entered.connect(progress)
			

func _get_property_list():
	var properties = []
	properties.append({
		"name": "step_name",
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
	if quest.step_names[quest.current_step] != step_name:
		print("didn't work")
		return
	quest.progress_quest()
	print("I progressed")
	QuestHandler.hud.quest_box.display_quest(quest)
	self.queue_free()
