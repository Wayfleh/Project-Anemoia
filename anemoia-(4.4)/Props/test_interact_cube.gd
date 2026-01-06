extends Node3D

var interacted_with: bool = false

var material = StandardMaterial3D.new()
@onready var box = %CSGBox3D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	%Interactable.focused.connect(_on_interactable_focused)
	%Interactable.interacted.connect(_on_interactable_interacted)
	%Interactable.unfocused.connect(_on_interactable_unfocused)

func _on_interactable_focused(interactor: Interactor) -> void:
	if not interacted_with:
		material.albedo_color = Color(0,1,1,1)
		box.set_material(material)

func _on_interactable_interacted(interactor: Interactor) -> void:
	if not interacted_with:
		material.albedo_color = Color(0,0,0,1)
		box.set_material(material)
		interacted_with = true

func _on_interactable_unfocused(interactor: Interactor) -> void:
	if not interacted_with:
		material.albedo_color = Color(1,1,1,1)
		box.set_material(material)

func on_save_game(saved_data: SavedData):
	saved_data.interactable_flags.append(interacted_with)

func on_load_game(saved_data: SavedData):
	interacted_with = saved_data.interactable_flags.pop_front()
	if interacted_with:
		material.albedo_color = Color(0,0,0,1)
		box.set_material(material)
		
