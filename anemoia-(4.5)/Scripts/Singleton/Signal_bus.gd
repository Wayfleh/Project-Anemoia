extends Node

signal dialogue_initiate(filename : JSON, talk_back: bool, trigger: Area3D)
signal dialogue_ended

func start_dialogue(filename: JSON, talk_back: bool, trigger: Area3D):
	dialogue_initiate.emit(filename, talk_back, trigger)

func end_dialogue():
	dialogue_ended.emit()
