extends Node

signal rolled
signal succeeded
signal one

signal stat_changed

@export var health := 7
@export var blood_pool := 10

@export var max_attribute := 5

@export var stat_list := {
	"strength" = 1,
	"dexterity" = 1,
	"stamina" = 1,
	"awareness" = 0,
	"brawl" = 0,
	"empathy" = 0,
	"intimidation" = 0,
	"subterfuge" = 0,
	"charisma" = 1,
	"manipulation" = 1,
	"appearance" = 0,
	"firearms" = 0,
	"larceny" = 0,
	"etiquette" = 0,
	"crafts" = 0,
	"stealth" = 0,
	"perception" = 1,
	"intelligence" = 1,
	"wits" = 1,
	"technology" = 0,
	"investigation" = 0,
	"academics" = 0,
	"occult" = 0,
	"politics" = 0,
	"humanity" = 7,
	"willpower" = 5,
}
@onready var base_personality : Resource = preload("res://Custom_Resources/Personalities.gd")
#array of resource instances of type Personalities
@export var personality_list = []

func _ready():
	var base = base_personality.new()
	load_personalities(base)

func load_personalities(value : Personalities):
	var file = FileAccess.open("res://Custom_Resources/Personalities.json", FileAccess.READ)
	var data = JSON.parse_string(file.get_as_text())
	for personality in data:
		var new_personality = value.instantiate(data[personality])
		personality_list.push_back(new_personality)
	for personality in personality_list:
		if personality.active == true:
			for stat in personality.stat_list:
				stats_inc_dec(personality.stat_list[stat], stat)
	for i in personality_list:
		print(i.to_string())

func add_personality(position : int):
	var personality = personality_list[position]
	if personality.active == true:
		return
	else:
		personality.active = true
		for stat in personality.stat_list:
			stats_inc_dec(personality.stat_list[stat], stat)
		stat_changed.emit()

func stats_inc_dec(value : int, stat : String) -> void:
	if value == 0 or stat_list[stat] == null:
		return
	else:
		stat_list[stat] = clampi(stat_list[stat] + value, 0, max_attribute)

func stats_inde_world(value: int, stat : String) -> void:
	stats_inc_dec(value, stat)
	stat_changed.emit()

func roll(attribute : String, ability : String, difficulty : int) -> int:
	var success := 0
	if self.get(attribute) == self.get(ability):
		return 0
	emit_signal("rolled")
	for n in (self.get(attribute) + self.get(ability)):
		var result := randi_range(1, 10)
		if result >= difficulty:
			success += 1
			emit_signal("succeeded")
		elif result == 1:
			success -= 1
			emit_signal("one")
	return success
