extends Resource

class_name Quest

var name : String
var completed : bool = false
var steps : Array[QuestSteps]
var current_step : int = 0

signal step_completed
signal quest_completed(n : String)

class QuestSteps:
	var name : String
	var completed : bool = false
	#condition will either be a quota or a bool, it is a bool if condition = 0
	var condition : int
	var progress : int = 0
	
	func _init(n : String, c : int):
		name = n
		condition = c
	
	func check_complete():
		if condition == 0:
			if progress >= 1:
				completed = true
		else:
			if progress >= condition:
				completed = true
	
	func inc_progress():
		if completed:
			return
		progress += 1
		check_complete()

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
