extends PanelContainer

@onready var close_button: Button = find_child("Close Debug")
@onready var add_personalities: Button = find_child("Add Personality")
@onready var print_personalities: Button = find_child("Print Current Personalities")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	close_button.pressed.connect(close_menu)
	add_personalities.pressed.connect(personality_test_add)
	print_personalities.pressed.connect(personality_test_print)

func close_menu() -> void:
	self.visible = false

func personality_test_add() -> void:
	for n in PlayerStats.personality_list.size():
		if PlayerStats.personality_list[n].active == false:
			PlayerStats.add_personality(n)

func personality_test_print() -> void:
	for i in PlayerStats.personality_list:
		print(i)
