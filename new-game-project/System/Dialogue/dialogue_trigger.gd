extends Area3D 
class_name DialogueTrigger

@export var dialogue : JSON
@export var talkable = false
@export var freeze_player : bool
@export var ID : String
@onready var already_talked : bool = false
@onready var triggered : bool = false
signal player_entered

func _ready():
	self.add_to_group("TalkingTriggers")
	SignalBus.dialogue_ended.connect(end_dialogue)

func on_area_entered(area: Area3D):
	if area is PlayerTrigger:
		start_dialogue()

func start_dialogue():
	if dialogue:
		SignalBus.start_dialogue(dialogue, talkable, self)
	else:
		already_talked = true
	player_entered.emit()
	triggered = true

func end_dialogue():
	if triggered:
		already_talked = true
