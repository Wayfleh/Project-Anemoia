extends Node

signal dialogue_initiate(filename : JSON)
signal dialogue_ended

func start_dialogue(filename: JSON, talk_back: bool):
	dialogue_initiate.emit(filename, talk_back)

func end_dialogue():
	dialogue_ended.emit()
