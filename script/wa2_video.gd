extends VideoStreamPlayer
class_name Wa2Video
func set_movie(path):
	var s=FFmpegVideoStream.new()
	s.file=path
	stream=s
	
