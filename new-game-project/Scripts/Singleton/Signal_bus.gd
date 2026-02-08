extends Node

#TODO put this shit in DialogueHandler, I don't need this stupid fucking script
#I just gotta update the two scripts to use the other class instead of this

signal dialogue_initiate(filename : JSON, talk_back: bool, trigger: Area3D)
signal dialogue_ended

#connects to UI/Dialogue/dialogue_box.gd
func start_dialogue(filename: JSON, talk_back: bool, trigger: Area3D):
	AudioManager.create_audio(SoundEffect.SOUND_EFFECT_TYPE.CHIME)
	dialogue_initiate.emit(filename, talk_back, trigger)

#connects to System/Dialogue/dialogue_trigger.gd
func end_dialogue():
	dialogue_ended.emit()
