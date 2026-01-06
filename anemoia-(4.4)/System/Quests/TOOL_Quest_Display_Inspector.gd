
extends Node

class_name InspectorQuestDisplay

@export var quest: Quest
var parent: QuestProgressor
var _step_names
var steps : Array[String]:
	get:
		return steps
	set(value):
		steps = value
		update_configuration_warnings()
		notify_property_list_changed()

func _ready():
	if Engine.is_editor_hint():
		parent = get_parent()
		if quest == null:
			return
		if parent.quest:
			quest = parent.quest

func _process(delta):
	if Engine.is_editor_hint():
		if parent.quest == quest && steps.size() < quest.steps.size():
			for step in quest.steps:
				steps.append(step.description)
		parent.step_name = _step_names


func _get_property_list():
	var properties = []
	properties.append({
		"name": "_step_names",
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
