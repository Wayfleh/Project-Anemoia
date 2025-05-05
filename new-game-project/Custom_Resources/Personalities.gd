class_name Personality extends AttributesAndAbilities

var patience := 5
var name : String
var resonance : String
var active : bool

signal impatient

func change_patience(value : int):
	patience = value
	patience = clampi(patience, 0, 5)
	if patience == 0:
		emit_signal("impatient")

func instantiate(json) -> Resource:
	var new_personality : Personality = self.duplicate()
	new_personality.name = json["name"]
	new_personality.resonance = json["resonance"]
	for stat_name in json["stats"]:
		new_personality.stat_list[stat_name] = json["stats"][stat_name]
	new_personality.active = json["active"]
	return new_personality

func _to_string() -> String:
	return str(patience) + name + resonance + str(active)
