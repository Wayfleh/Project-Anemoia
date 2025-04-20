class_name BaseStats
extends Resource

signal stats_changed

@export var health := 7
@export var blood_pool := 10
@export var humanity := 7
@export var willpower := 5
@export var strength := 1
@export var dexterity := 1
@export var stamina := 1

var blood := 1 

func set_blood(value : int) -> void:
	blood = clampi(value, 0, blood_pool)

func base_stat_inc_dec(value : int, stat : String) -> bool:
	if value == 0 or self.get(stat) == null:
		return false
	self.set(stat, self.get(stat) + value)
	emit_signal("stats_changed")
	return true

func create_instance() -> Resource:
	var instance: BaseStats = self.duplicate()
	instance.health = 7
	instance.blood = blood_pool
	instance.humanity = 7
	instance.willpower = 5
	instance.strength = 1
	instance.dexterity = 1
	instance.stamina = 1
	return instance
