class_name Personalities extends Resource

@export var patience := 5
@export var name : String
@export var resonance : String
@export var active : bool
@export var stat_list : Dictionary[String, int] = {}

signal impatient

func change_patience(value : int):
	patience = value
	patience = clampi(patience, 0, 5)
	if patience == 0:
		emit_signal("impatient")

func instantiate(json) -> Resource:
	var new_personality : Personalities = self.duplicate()
	new_personality.name = json["name"]
	new_personality.resonance = json["resonance"]
	for stat_name in json["stats"]:
		new_personality.stat_list[stat_name] = (json["stats"][stat_name] as int)
	new_personality.active = json["active"]
	return new_personality

func _to_string() -> String:
	var stats = " "
	for stat in stat_list:
		stats += stat + " " + str(stat_list[stat]) + " "
	return str(patience) + "; Name: " + name + "; Resonance: " + resonance + "; Active: " + str(active) + "; Stats: (" + stats + ")"
