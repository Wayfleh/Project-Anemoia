class_name TextBox extends Label

@onready var l = %Label

func _ready():
	autowrap_mode = 2
	size_flags_horizontal = Control.SIZE_EXPAND_FILL
