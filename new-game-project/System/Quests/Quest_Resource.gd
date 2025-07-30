extends Resource

class_name Quest

@export var name : String
@export_multiline var description : String
var completed : bool = false
@export var steps : Array[QuestSteps] = []
var current_step : int = 0

signal step_completed
signal quest_completed(n : String)



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
