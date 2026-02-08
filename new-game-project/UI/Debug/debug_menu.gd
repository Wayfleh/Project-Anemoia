extends PanelContainer

@onready var close_button: Button = find_child("Close Debug")
@onready var add_personalities: Button = find_child("Add Personality")
@onready var print_personalities: Button = find_child("Print Current Personalities")
@onready var change_map: Button = find_child("Change Map")

@onready var maps_menu = %"Maps Menu"
var maps_path: String = "res://Maps"
@export var maps: Array[Node]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_load_map_buttons()
	close_button.pressed.connect(close_menu)
	add_personalities.pressed.connect(personality_test_add)
	print_personalities.pressed.connect(personality_test_print)
	change_map.pressed.connect(_show_maps)

func _show_maps() -> void:
	maps_menu.visible = true

func _load_map_buttons() -> void:
	for map in DirAccess.get_files_at(maps_path):
		var button = Button.new()
		button.text = map
		button.pressed.connect(_load_map.bind(map))
		maps_menu.get_children()[0].add_child(button)

func _load_map(map: String) -> void:
	get_tree().get_first_node_in_group("World").change_the_world(map)

func close_menu() -> void:
	self.visible = false
	maps_menu.visible = false

func personality_test_add() -> void:
	for n in PlayerStats.personality_list.size():
		if PlayerStats.personality_list[n].active == false:
			PlayerStats.add_personality(n)

func personality_test_print() -> void:
	for i in PlayerStats.personality_list:
		print(i)
