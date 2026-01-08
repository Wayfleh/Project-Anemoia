extends MenuScreen

@onready var _completed_log = %CompletedLogMargin/CompletedLog/List
@onready var _progress_log = %ProgressLogMargin/ProgressLog/List

@onready var _quest_step_list = %QuestDisplay/Quest/StepList
@onready var quest_label_scn = preload("res://UI/HUD/QuestLabel.tscn")
@onready var _step_des: Array[QuestLabel] = []

@onready var _quest_name = %QuestDisplay/Quest/QuestName

@onready var resume_button = %Resume

func _ready() -> void:
	$AnimationPlayer.play("RESET")
	resume_button.pressed.connect(resume)
	_populate_logs()
	QuestHandler.quest_added.connect(_populate_logs)
	QuestHandler.quest_complete.connect(_populate_logs)

func resume():
	get_tree().paused = false
	$AnimationPlayer.play("Unpause")
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	_clear_display_quest()
	_resetUIState()

func pause():
	get_tree().paused = true
	$AnimationPlayer.play("Pause")
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	_populate_logs()
	if QuestHandler.current_quest != null:
		_display_quest(QuestHandler.current_quest)

func _populate_logs() -> void:
	for button in _completed_log.get_children():
		button.queue_free()
	for button in _progress_log.get_children():
		button.queue_free()
	for quest in QuestHandler.completed_log:
		var comp_button := Button.new()
		comp_button.focus_mode = Control.FOCUS_NONE
		comp_button.text = quest.name
		comp_button.pressed.connect(_display_quest.bind(quest))
		_completed_log.add_child(comp_button)
	for quest in QuestHandler.quest_log:
		var prog_button := Button.new()
		prog_button.focus_mode = Control.FOCUS_NONE
		prog_button.text = quest.name
		prog_button.pressed.connect(_display_quest.bind(quest))
		_progress_log.add_child(prog_button)

func _display_quest(quest : Quest) -> void:
	if %QuestDisplay.visible:
		_clear_display_quest() #closes display when button is pressed again
		return
	_clear_display_quest()
	_quest_name.text = "[b]" + quest.name.to_upper()
	for step in quest.steps:
		var curr_step_des : QuestLabel = quest_label_scn.instantiate()
		_step_des.append(curr_step_des)
		curr_step_des.bbcode_enabled = true
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
		_quest_step_list.add_child(curr_step_des)
	%QuestDisplay.show()

func _clear_display_quest() -> void:
	%QuestDisplay.hide()
	if _step_des.is_empty():
		return
	for step in _step_des:
		_quest_step_list.remove_child(step)
		step.queue_free()
	_step_des.clear()
