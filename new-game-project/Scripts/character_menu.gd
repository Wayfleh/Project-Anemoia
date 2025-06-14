extends Control

@export var char_stats : AttributesAndAbilities

@onready var resume_button: Button = find_child("Resume")
@onready var revert_button: Button = find_child("Revert")
@onready var accept_changes_button: Button = find_child("Accept Changes")
#FUCKKKKK
@onready var strength_button : Button = find_child("Strength")
@onready var dexterity_button : Button = find_child("Dexterity")
@onready var stamina_button : Button = find_child("Stamina")
@onready var awareness_button : Button = find_child("Awareness")
@onready var brawl_button : Button = find_child("Brawl")
@onready var empathy_button : Button = find_child("Empathy")
@onready var intimidation_button : Button = find_child("Intimidation")
@onready var subterfuge_button : Button = find_child("Subterfuge")
@onready var charisma_button : Button = find_child("Charisma")
@onready var manipulation_button : Button = find_child("Manipulation")
@onready var appearance_button : Button = find_child("Appearance")
@onready var firearms_button : Button = find_child("Firearms")
@onready var larceny_button : Button = find_child("Larceny")
@onready var etiquette_button : Button = find_child("Etiquette")
@onready var crafts_button : Button = find_child("Crafts")
@onready var stealth_button : Button = find_child("Stealth")
@onready var perception_button : Button = find_child("Perception")
@onready var intelligence_button : Button = find_child("Intelligence")
@onready var wits_button : Button = find_child("Wits")
@onready var technology_button : Button = find_child("Technology")
@onready var investigation_button : Button = find_child("Investigation")
@onready var academics_button : Button = find_child("Academics")
@onready var occult_button : Button = find_child("Occult")
@onready var politics_button : Button = find_child("Politics")
@onready var humanity_button : Button = find_child("Humanity")
@onready var willpower_button : Button = find_child("Willpower")
#FAAUUUUUWWWCCCKKKKK
@onready var strength_button_container : HBoxContainer = find_child("StrengthContainer")
@onready var dexterity_button_container : HBoxContainer = find_child("DexterityContainer")
@onready var stamina_button_container : HBoxContainer = find_child("StaminaContainer")
@onready var awareness_button_container : HBoxContainer = find_child("AwarenessContainer")
@onready var brawl_button_container : HBoxContainer = find_child("BrawlContainer")
@onready var empathy_button_container : HBoxContainer = find_child("EmpathyContainer")
@onready var intimidation_button_container : HBoxContainer = find_child("IntimidationContainer")
@onready var subterfuge_button_container : HBoxContainer = find_child("SubterfugeContainer")
@onready var charisma_button_container : HBoxContainer = find_child("CharismaContainer")
@onready var manipulation_button_container : HBoxContainer = find_child("ManipulationContainer")
@onready var appearance_button_container : HBoxContainer = find_child("AppearanceContainer")
@onready var firearms_button_container : HBoxContainer = find_child("FirearmsContainer")
@onready var larceny_button_container : HBoxContainer = find_child("LarcenyContainer")
@onready var etiquette_button_container : HBoxContainer = find_child("EtiquetteContainer")
@onready var crafts_button_container : HBoxContainer = find_child("CraftsContainer")
@onready var stealth_button_container : HBoxContainer = find_child("StealthContainer")
@onready var perception_button_container : HBoxContainer = find_child("PerceptionContainer")
@onready var intelligence_button_container : HBoxContainer = find_child("IntelligenceContainer")
@onready var wits_button_container : HBoxContainer = find_child("WitsContainer")
@onready var technology_button_container : HBoxContainer = find_child("TechnologyContainer")
@onready var investigation_button_container : HBoxContainer = find_child("InvestigationContainer")
@onready var academics_button_container : HBoxContainer = find_child("AcademicsContainer")
@onready var occult_button_container : HBoxContainer = find_child("OccultContainer")
@onready var politics_button_container : HBoxContainer = find_child("PoliticsContainer")
@onready var humanity_button_container : HBoxContainer = find_child("HumanityContainer")
@onready var willpower_button_container : HBoxContainer = find_child("WillpowerContainer")

var unfilled_dot = preload("res://Assets/Textures/unfilled_dot.png")
var filled_dot = preload("res://Assets/Textures/filled_dot.png")
var temp_dot = preload("res://Assets/Textures/temp_dot.png")
var temp_dict = {}


func _ready():
	$AnimationPlayer.play("RESET")
	resume_button.pressed.connect(resume)
	revert_button.pressed.connect(revert)
	accept_changes_button.pressed.connect(accept_changes)
	for stat_name in char_stats.stat_list:
		var button = self.get("%s_button" % stat_name)
		button.pressed.connect(_on_pressed.bind(stat_name))
		var rating = char_stats.stat_list[stat_name]
		var container = self.get("%s_button_container" % stat_name)
		for n in container.get_child_count():
			if rating > n:
				container.get_child(n).texture = filled_dot
			else:
				container.get_child(n).texture = unfilled_dot

func update_stats(value : int, stats : AttributesAndAbilities, stat : String, temp : bool) -> void:
	var container = self.get("%s_button_container" % stat)
	for n in container.get_child_count():
		if stats.get(stat) > n:
			container.get_child(n).texture = filled_dot
		elif (stats.get(stat) + value) > n && temp:
			container.get_child(n).texture = temp_dot
		else:
			container.get_child(n).texture = unfilled_dot
	

func _on_pressed(stat):
	if !(temp_dict.has(stat)):
		temp_dict[stat] = 0
	temp_dict[stat] += 1
	update_stats(temp_dict[stat], char_stats, stat, true)
	
func revert():
	for stat_name in temp_dict:
		update_stats(0, char_stats, stat_name, false)
	temp_dict.clear()

func accept_changes():
	for stat_name in temp_dict:
		char_stats.stats_inc_dec(temp_dict[stat_name], stat_name)
		update_stats(0, char_stats, stat_name, false)
	
func resume():
	get_tree().paused = false
	$AnimationPlayer.play("Unpause")
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	revert()

func pause():
	get_tree().paused = true
	$AnimationPlayer.play("Pause")
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	
