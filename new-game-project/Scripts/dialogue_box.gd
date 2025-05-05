extends Control

@export var debug_json: JSON
@onready var state = {}

@onready var choice_button_scn = preload("res://Scenes/UI/Dialogue/choice_button.tscn")
@onready var text_box_scn = preload("res://Scenes/UI/Dialogue/text_box.tscn")

@onready var scroll = $Scroll
@onready var scrollbar = scroll.get_v_scroll_bar()

var text_history : Array[Label] = []
var choice_buttons : Array[Button] = []

func _ready():
	($EzDialogue as EzDialogue).start_dialogue(debug_json, state)

func clear_dialogue_box():
	for label in text_history:
		$Scroll/Margin/History.remove_child(label)
	clear_choices()
	text_history = []

func clear_choices():
	for choice in choice_buttons:
		$Scroll/Margin/History.remove_child(choice)
	choice_buttons = []

func add_text(text: String):
	var label : TextBox = text_box_scn.instantiate()
	$Scroll/Margin/History.add_child(label)
	label["theme_override_font_sizes/font_size"] = 20
	#TODO Create a theme file so you can replace this shit. Optomization-style
	label.text = text + "\n"

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
	$Scroll/Margin/History.add_child(button_obj)
	scroll.scroll_vertical = scrollbar.max_value

func _on_choice_selected(choice_index: int):
	if !choice_buttons[choice_index].continue_button:
		var old_choice : TextBox = text_box_scn.instantiate()
		$Scroll/Margin/History.add_child(old_choice)
		old_choice.text = "You: " + choice_buttons[choice_index].text + "\n"
		old_choice["theme_override_font_sizes/font_size"] = 20
		old_choice["theme_override_colors/font_color"] = Color("orange")
	clear_choices()
	($EzDialogue as EzDialogue).next(choice_index)


func _on_ez_dialogue_dialogue_generated(response: DialogueResponse) -> void:
	print(response.text)
	response.text = response.text.replace("\n", " ")
	add_text(response.text)
	if response.choices.is_empty() && !response.eod_reached:
		add_choice("Continue ->", true)
	else:
		for choice in response.choices:
			add_choice(choice, false)
