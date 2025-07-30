extends Resource
class_name QuestSteps

#@export var name : String
@export var description: String
var completed : bool = false
#condition will either be a quota or a bool, it is a bool if condition = 0
@export var condition : int
var progress : int = 0

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
