extends Control

@onready var state = {}

@onready var choice_button_scn = preload("res://Scenes/UI/Dialogue/choice_button.tscn")
@onready var text_box_scn = preload("res://Scenes/UI/Dialogue/text_box.tscn")

@onready var scroll = $Scroll
@onready var scrollbar = scroll.get_v_scroll_bar()
@onready var insideMind = true
@onready var end_of_dialogue_reached = false

var text_history_I : Array[TextBox] = []
var text_history_E : Array[TextBox] = []
var choice_buttons : Array[ChoiceButton] = []

func _ready():
	$EzDialogue.custom_signal_received.connect(_on_custom_signal_received)
	#$EzDialogue.end_of_dialogue_reached.connect(clear_dialogue_box)
	SignalBus.dialogue_initiate.connect(initiate_dialogue)

func initiate_dialogue(filename: JSON, talk_back: bool):
	for personality in PlayerStats.personality_list:
		state["%s" % personality.name] = personality.active
	if talk_back:
		%DialogueBoxTwo.visible = true
	($EzDialogue as EzDialogue).start_dialogue(filename, state)

func _on_custom_signal_received(value: String):
	var param = value.split(",")
	if param[0] == "internal":
		if param[1] == "false":
			insideMind = false
		else:
			insideMind = true

func clear_dialogue_box():
	print("debug")
	for label in text_history_E:
		print("External remove label")
		%External/History.remove_child(label)
		label.queue_free()
	for label in text_history_I:
		print("Internal remove label")
		%Internal/History.remove_child(label)
		label.queue_free()
	clear_choices()
	text_history_I = []
	text_history_E = []
	insideMind = true
	SignalBus.end_dialogue()
	%DialogueBoxTwo.visible = false
	end_of_dialogue_reached = false
	

func clear_choices():
	for choice in choice_buttons:
		%Internal/History.remove_child(choice)
	choice_buttons = []

func add_text(text: String):
	var label : TextBox = text_box_scn.instantiate()
	if insideMind:
		%Internal/History.add_child(label)
		text_history_I.append(label)
	else:
		%External/History.add_child(label)
		text_history_E.append(label)
	#label["theme_override_font_sizes/font_size"] = 20
	#TODO Create a theme file so you can replace this shit. Optomization-style
	label.text = text + "\n\n"
	if end_of_dialogue_reached == true:
		clear_dialogue_box()
	#TODO when end of dialogue is a page break or it ends right after making an empty choice,
	#   make a timer that keeps the dialogue on history until the timer ends or until another 
	#   dialogue is initiated. Timer should restart when player scrolls or otherwise touches
	#   the dialogue box

func add_choice(choice_text: String, page_break: bool):
	var button_obj: ChoiceButton = choice_button_scn.instantiate()
	button_obj.choice_index = choice_buttons.size()
	choice_buttons.push_back(button_obj)
	button_obj.text = choice_text
	button_obj.choice_selected.connect(_on_choice_selected)
	if page_break:
		button_obj.continue_button = true
		var shortcut : Shortcut = Shortcut.new()
		var event : InputEventKey = InputEventKey.new()
		event.keycode = KEY_ENTER
		shortcut.events.push_back(event)
		button_obj.shortcut = shortcut
	%Internal/History.add_child(button_obj)

func _on_choice_selected(choice_index: int):
	if !choice_buttons[choice_index].continue_button:
		var old_choice : TextBox = text_box_scn.instantiate()
		%Internal/History.add_child(old_choice)
		text_history_I.append(old_choice)
		old_choice.text = "You: " + choice_buttons[choice_index].text + "\n\n"
		old_choice["theme_override_font_sizes/font_size"] = 20
		old_choice["theme_override_colors/font_color"] = Color("orange")
	clear_choices()
	($EzDialogue as EzDialogue).next(choice_index)


func _on_ez_dialogue_dialogue_generated(response: DialogueResponse) -> void:
	print(response.text)
	if response.choices.is_empty() && response.eod_reached:
		end_of_dialogue_reached = true
	response.text = response.text.replace("\n", " ")
	response.text = response.text.replace("\\n", "\n")
	add_text(response.text)
	if response.choices.is_empty() && !response.eod_reached:
		add_choice("Continue ->", true)
	else:
		for choice in response.choices:
			add_choice(choice, false)
