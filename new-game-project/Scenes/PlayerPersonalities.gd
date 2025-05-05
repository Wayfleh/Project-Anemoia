extends Node

@export var base_personality : Personality 
@onready var personality_list = []

func _ready():
	load_personalities(base_personality)

func load_personalities(value : Personality):
	var file = FileAccess.open("res://Custom_Resources/Personalities.json", FileAccess.READ)
	var data = JSON.parse_string(file.get_as_text())
	for personality in data:
		var new_personality = value.instantiate(data[personality])
		personality_list.push_back(new_personality)
	for i in personality_list:
		print(i.to_string())
	
