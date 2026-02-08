class_name SavedData
extends Resource

const SAVE_GAME_PATH := "user://save.tres"

#This thing will be used to detect old saves and update the data
@export var version := 0

@export var player_stat_list = {}
@export var personalities: Resource = preload("res://Custom_Resources/Personalities.gd")
@export var personality_list = []
@export var player_position: Vector3
@export var interactable_flags: Array[bool] = []
@export var quest_log: Array[Quest] = []
@export var current_quest: Quest = null
