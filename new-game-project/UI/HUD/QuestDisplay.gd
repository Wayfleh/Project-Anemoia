extends ColorRect

class_name QuestDisplay

@onready var quest_label_scn = preload("res://UI/HUD/QuestLabel.tscn")
@onready var step_des : Array[QuestLabel]

func display_quest(quest : Quest) -> void:
	for label in $TextContainer.get_children():
		$TextContainer.remove_child(label)
		label.queue_free()
	var quest_name : QuestLabel = quest_label_scn.instantiate()
	quest_name.text = "[b]" + quest.name + "[/b]"
	$TextContainer.add_child(quest_name)
	for i in quest.current_step + 1:
		var curr_step_des : QuestLabel = quest_label_scn.instantiate()
		$TextContainer.add_child(curr_step_des)
		step_des.append(curr_step_des)
		while step_des.size() > 3:
			step_des.pop_front()
		curr_step_des.bbcode_enabled = true
		if quest.completed:
			curr_step_des.text = "[color=green] Quest Finished [/color]"
			return
		var step = quest.steps[i]
		if step.condition == 0:
			if step.completed:
				curr_step_des.text = "[color=blue] %s: 1/1[/color]" % step.description
			else:
				curr_step_des.text = step.description + ": 0/1"
		else:
			if step.completed:
				curr_step_des.text = "[color=blue] %s: %s/%s[/color]" % [step.description, step.condition, step.condition]
			else:
				curr_step_des.text = "%s : %s/%s" % [step.description, step.progress, step.condition]
		
		
