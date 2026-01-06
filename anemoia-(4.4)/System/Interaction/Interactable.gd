extends Area3D

class_name Interactable

#Emitted when player looks at me
signal focused(interactor: Interactor)
#Emitted when player stops looking at me
signal unfocused(interactor: Interactor)
#Emitted when player interacts with me
signal interacted(interactor: Interactor)
