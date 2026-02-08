extends Node

#add this node to an Area3D node (Trigger, Talkable, Interactable)
@export var quest : Quest
enum types {TRIGGER, TALK, INTERACT}
@export var quest_type : types
@onready var parent

func _ready() -> void:
	parent = get_parent()
	match quest_type:
		types.TRIGGER:
			(parent as DialogueTrigger).player_entered.connect(accept_quest)
		types.TALK:
			(parent as Talkable).quest_accept.connect(accept_quest)
			print("got it")
		types.INTERACT:
			(parent as Interactable).interacted.connect(accept_quest)

func accept_quest() -> void:
	if QuestHandler.quest_log.has(quest) or QuestHandler.completed_log.has(quest):
		self.queue_free()
		return
	QuestHandler.add_quest(quest)
	QuestHandler.hud.quest_box.display_quest(quest)
	self.queue_free()
