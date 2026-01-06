extends Resource

class_name Quest

@export var name : String
@export_multiline var description : String
var completed : bool = false
var step_names : Array[String] = ["Default"]
@export var steps : Array[QuestSteps] = []:
	set(value):
		steps = value
		for step in value:
			step_names.append(step.description + "")
		notify_property_list_changed()
var current_step : int = 0


signal step_completed
signal quest_completed(n : String)


var _list: String:
	set(value):
		_list = value
		notify_property_list_changed()



func check_complete():
	if current_step < steps.size():
		return
	completed = true
	quest_completed.emit(name)

func progress_quest():
	if completed:
		return
	steps[current_step].inc_progress()
	if steps[current_step].completed:
		current_step += 1
		step_completed.emit()
	check_complete()
