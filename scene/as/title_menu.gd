extends Control
@onready var titile_menu=$TitleMenu
@onready var inital_start=$InitalStart
@onready var sonw=$GPUParticles2D
@onready var animation_player:AnimationPlayer=$AnimationPlayer
#func open():
	##await Globals.video_command("mv00")
	##await Globals.video_command("mv02")
	##await Globals.video_command(10)
	#Globals.cur_page=1
	#show()
	#sonw.hide()
	#animation_player.play("RESET")
	#await animation_player.animation_finished
	##animation_player.play("logo")
	##await animation_player.animation_finished
	#Sound.play_bgm(31)
	#animation_player.play("open")
	#await animation_player.animation_finished
	#sonw.restart()
	#sonw.show()
func _gui_input(event):

	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
		if animation_player.current_animation=="logo":
			animation_player.advance(3)
		elif animation_player.current_animation=="open":
			animation_player.advance(2.5)
#
#
func _on_start_button_down():
	titile_menu.hide()
	inital_start.show()
	#
#
#
#func _on_start_1_button_down():
	#await  close()
	#Ws.load("6001")
	#Globals.game_state=1
	#Globals.cur_page=0
#func _on_start_2_button_down():
	#await  close()
	#Ws.load("6101")
	#Globals.game_state=1
	#Globals.cur_page=0
#
#func _on_quit_button_down():
	#get_tree().quit()
#
#
#func _on_start_back_button_down():
	#inital_start.hide()
	#titile_menu.show()
#
#
#func _on_load_button_down():
	#Globals.load_page.open()
	#
func _ready():
	animation_player.play("logo")
	await animation_player.animation_finished
	animation_player.play("open")
	Wa2Func.PlayBgm(31,false,255)
	#await get_tree().physics_frame
	#Globals.load_page.connect("load_file",func (no:int):
		#animation_player.play("close")
		#await  close()
		#Globals.cur_page=0
		#Globals.load_file(no)
		#)
func close():
	Wa2Func.StopBgm(0);
	animation_player.play("close")
	await animation_player.animation_finished
	hide()
#
#
#func _on_ic_button_down():
	#await  close()
	#Ws.load("1001")
	#Globals.game_state=1
	#Globals.cur_page=0
#
#
#func _on_cc_button_down():.mn			 
	#await  close()
	#Ws.load("2001")
	#Globals.game_state=1
	#Globals.cur_page=0
#
#
#func _on_coda_button_down():
	#pass # Replace with function body.


func _on_as_1_button_down() -> void:
	await  close()
	Wa2Func.LoadScript("6001",0)


func _on_start_back_button_down() -> void:
	inital_start.hide()
	titile_menu.show()
