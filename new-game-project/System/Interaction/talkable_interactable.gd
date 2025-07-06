extends Interactable

class_name Talkable

@onready var dialogue : JSON = get_parent().dialogue
@onready var talk_back = get_parent().talk_back
@onready var freeze_player = get_parent().freeze_player
