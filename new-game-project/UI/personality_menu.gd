extends MenuScreen

var button = preload("res://UI/PersonalityMenu/PersonalityButton.tscn")
@onready var resume_button: Button = find_child("Resume")
@export var player: Player
@onready var InfoVBox = %Info/InfoHolder
@onready var last_button_pressed_index : int = -1

func _ready():
	$AnimationPlayer.play("RESET")
	resume_button.pressed.connect(resume)
	player = get_tree().get_nodes_in_group("Player")[0]

func resume():
	get_tree().paused = false
	$AnimationPlayer.play("Unpause")
	if (player.current_state as StateMachineState) is not Frozen:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	for n in %Row1.get_children():
		%Row1.remove_child(n)
		n.queue_free()
	%Info.visible = false
	clear_info_box()
	_resetUIState()

func pause():
	load_personalities()
	get_tree().paused = true
	$AnimationPlayer.play("Pause")
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)

func load_personalities():
	print(PlayerStats.personality_list.size())
	for index in PlayerStats.personality_list.size():
		var personality = PlayerStats.personality_list[index]
		if personality.active:
			var pers_button: PersonalityButton = button.instantiate()
			var image_path: String = "res://Assets/Portraits/Personalities/%s.png"
			var texture : Texture2D
			if !load(image_path % personality.name):
				print("Error loading image: ", image_path % personality.name)
				texture = load("res://Assets/Portraits/Personalities/Jack.jpg")
			else:
				texture = load(image_path % personality.name)
			pers_button.texture_normal = texture
			#TODO make these different textures
			pers_button.texture_hover = texture
			pers_button.texture_pressed = texture
			%Row1.add_child(pers_button)
			pers_button.pressed.connect(show_personality.bind(index))

func show_personality(index: int):
	var pers_name : Label = InfoVBox.find_child("Name")
	var resonance : Label = InfoVBox.find_child("Resonance")
	var stat_list : VBoxContainer = InfoVBox.find_child("Stats")
	var personality : Personalities = PlayerStats.personality_list[index]
	if %Info.visible:
		clear_info_box()
		if last_button_pressed_index == index:
			%Info.visible = false
			return
	%Info.visible = true
	last_button_pressed_index = index
	pers_name.text = personality.name
	resonance.text = personality.resonance
	for stat in personality.stat_list:
		var stat_label = Label.new()
		var rating = personality.stat_list[stat]
		stat_label.text = stat + ": " + str(rating)
		stat_list.add_child(stat_label)

func clear_info_box():
	var pers_name : Label = InfoVBox.find_child("Name")
	var resonance : Label = InfoVBox.find_child("Resonance")
	var stat_list : VBoxContainer = InfoVBox.find_child("Stats")
	pers_name.text = ""
	resonance.text = ""
	for n in stat_list.get_children():
		stat_list.remove_child(n)
		n.queue_free()
