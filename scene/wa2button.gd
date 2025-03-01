extends TextureButton
class_name Wa2Button
@export var hover_se:AudioStream
@export var click_se:AudioStream
#@export var buttton_type:Consts.ButtonType
#@export var idx:=0
func _ready():
	connect("button_down",func ():
		if !disabled:
			Wa2Func.PlaySysSe(click_se)
		)
	#connect("button_down",func ():
		#Sound.play_sys_se(click_se)
		#Events.emit_signal("push_event",{
			#"type":Consts.EventType.BUTTON_DOWN,
			#"button_type":buttton_type,
			#"idx":idx
		#})
		#)
#InputEvent
func _gui_input(event):
	pass
	#if disabled:
		#Globals.main.propagate_call("_gui_input",[event])
