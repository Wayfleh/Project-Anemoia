extends Node

var file = "res://Custom_Resources/TestPersonality.json"

func _ready():
	load_file(file)

func load_file(file):
	var f = File.new()
	f.open(file, File.READ)
