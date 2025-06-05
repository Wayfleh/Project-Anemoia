#Saved in user://settings.ini
#On Windows, it's stored in %APPDATA%\Godot\app_userdata\Anemoia
#On macOS, it's stored in ~/Library/Application Support/Godot/app_userdata/Anemoia
extends Node

var config = ConfigFile.new()
const SETTINGS_FILE_PATH = "user://settings.ini"

func _ready():
	if !FileAccess.file_exists(SETTINGS_FILE_PATH):
		config.set_value("keybinding", "up", "W")
		config.set_value("keybinding", "down", "S")
		config.set_value("keybinding", "left", "A")
		config.set_value("keybinding", "right", "D")
		config.set_value("keybinding", "jump", "Space")
		config.set_value("keybinding", "sprint", "Shift")
		config.set_value("keybinding", "char_menu_button", "Tab")
		config.set_value("keybinding", "Free Camera", "F")
	if FileAccess.file_exists("user://data.cfg"):
		config.load("user://data.cfg")
