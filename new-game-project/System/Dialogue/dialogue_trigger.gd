extends Area3D 
class_name DialogueTrigger

@export var dialogue : JSON
@export var talkable = false
@export var freeze_player : bool
@export var ID : String
@onready var already_talked : bool = false
@onready var triggered : bool = false

func _ready():
	self.add_to_group("TalkingTriggers")
	SignalBus.dialogue_ended.connect(end_dialogue)

func start_dialogue():
	SignalBus.start_dialogue(dialogue, talkable, self)
	triggered = true

func end_dialogue():
	if triggered:
		already_talked = true
