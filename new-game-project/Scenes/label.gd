extends Label

@onready var l = %Label

func _ready():
	l.anchor_left = 0
	l.anchor_right = 1
	l.set_autowrap_mode(true)
