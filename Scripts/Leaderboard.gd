extends Node

# Use this game API key if you want to test it with a functioning leaderboard
# "987dbd0b9e5eb3749072acc47a210996eea9feb0"
var game_API_key = "7882fa319d13e89e9346d700fb647d4736539f12"
var development_mode = true
var leaderboard_key = "ScoreKey"
var session_token = ""

# HTTP Request node can only handle one call per node
var auth_http = HTTPRequest.new()
var leaderboard_http = HTTPRequest.new()
var submit_score_http = HTTPRequest.new()
var set_name_http = HTTPRequest.new()
var get_name_http = HTTPRequest.new()

signal name_gotten(name)
signal leaderboard_gotten(ranks, names, scores)
signal score_uploaded()

func _ready():
	_authentication_request()

#func _process(_delta):
#	if(Input.is_action_just_pressed("ui_up")):
#		score += 1
#		print("CurrentScore:"+str(score))
#
#	if(Input.is_action_just_pressed("ui_down")):
#		score -= 1
#		print("CurrentScore:"+str(score))
#
#	# Upload score when pressing enter
#	if(Input.is_action_just_pressed("ui_accept")):
#		_upload_score(score)
#
#	# Get score when pressing spacebar
#	if(Input.is_action_just_pressed("ui_select")):
#		_get_leaderboards()


func _authentication_request():
	# Check if a player session has been saved
	var player_session_exists = false
	var file = File.new()
	file.open("user://LootLocker.data", File.READ)
	var player_identifier = file.get_as_text()
	file.close()
	if(player_identifier.length() > 1):
		player_session_exists = true
		
	## Convert data to json string:
	var data = { "game_key": game_API_key, "game_version": "0.0.0.1", "development_mode": true }
	
	# If a player session already exists, send with the player identifier
	if(player_session_exists == true):
		data = { "game_key": game_API_key, "player_identifier":player_identifier, "game_version": "0.0.0.1", "development_mode": true }
	
	# Add 'Content-Type' header:
	var headers = ["Content-Type: application/json"]
	
	# Create a HTTPRequest node for authentication
	auth_http = HTTPRequest.new()
	add_child(auth_http)
	auth_http.connect("request_completed", self, "_on_authentication_request_completed")
	# Send request
	auth_http.request("https://api.lootlocker.io/game/v2/session/guest", headers, true, HTTPClient.METHOD_POST, to_json(data))
	# Print what we're sending, for debugging purposes:
	print(data)


func _on_authentication_request_completed(result, response_code, headers, body):
	var json = JSON.parse(body.get_string_from_utf8())
	# Save player_identifier to file
	var file = File.new()
	file.open("user://LootLocker.data", File.WRITE)
	file.store_string(json.result.player_identifier                                                                                        )
	file.close()
	
	# Save session_token to memory
	session_token = json.result.session_token
	
	# Print server response
	print(json.result)
	
	# Clear node
	auth_http.queue_free()
	# Get leaderboards
	# _get_leaderboards()


func _get_leaderboards():
	print("Getting leaderboards")
	var url = "https://api.lootlocker.io/game/leaderboards/"+leaderboard_key+"/list?count=25"
	var headers = ["Content-Type: application/json", "x-session-token:"+session_token]
	
	# Create a request node for getting the highscore
	leaderboard_http = HTTPRequest.new()
	add_child(leaderboard_http)
	leaderboard_http.connect("request_completed", self, "_on_leaderboard_request_completed")
	# Send request
	leaderboard_http.request(url, headers, true, HTTPClient.METHOD_GET, "")


func _on_leaderboard_request_completed(result, response_code, headers, body):
	var json = JSON.parse(body.get_string_from_utf8())
	
	# Print data
	print(json.result)
	
	# # Formatting as a leaderboard
	# var leaderboardFormatted = ""
	# for n in json.result.items.size():
	# 	leaderboardFormatted += str(json.result.items[n].rank)+str(". ")
	# 	leaderboardFormatted += str(json.result.items[n].player.id)+str(" - ")
	# 	leaderboardFormatted += str(json.result.items[n].score)+str("\n")
	
	var ranks = []
	var names = []
	var scores = []
	for n in json.result.items.size():
		ranks.append(json.result.items[n].rank)
		names.append(json.result.items[n].player.name)
		scores.append(json.result.items[n].score)

	emit_signal("leaderboard_gotten", ranks, names, scores);

	# # Print the formatted leaderboard to the console
	# print(leaderboardFormatted)
	
	# Clear node
	leaderboard_http.queue_free()


func _upload_score(var score):
	var data = { "score": str(score) }
	var headers = ["Content-Type: application/json", "x-session-token:"+session_token]
	submit_score_http = HTTPRequest.new()
	add_child(submit_score_http)
	submit_score_http.connect("request_completed", self, "_on_upload_score_request_completed")
	# Send request
	submit_score_http.request("https://api.lootlocker.io/game/leaderboards/"+leaderboard_key+"/submit", headers, true, HTTPClient.METHOD_POST, to_json(data))
	# Print what we're sending, for debugging purposes:
	print(data)

func _change_player_name(var newName):
	print("Changing player name")
	
	# use this variable for setting the name of the player
	var player_name = newName
	
	var data = { "name": str(player_name) }
	var url =  "https://api.lootlocker.io/game/player/name"
	var headers = ["Content-Type: application/json", "x-session-token:"+session_token]
	
	# Create a request node for getting the highscore
	set_name_http = HTTPRequest.new()
	add_child(set_name_http)
	set_name_http.connect("request_completed", self, "_on_player_set_name_request_completed")
	# Send request
	set_name_http.request(url, headers, true, HTTPClient.METHOD_PATCH, to_json(data))
	
func _on_player_set_name_request_completed(result, response_code, headers, body):
	var json = JSON.parse(body.get_string_from_utf8())
	
	# Print data
	print(json.result)
	set_name_http.queue_free()

func _get_player_name():
	print("Getting player name")
	var url = "https://api.lootlocker.io/game/player/name"
	var headers = ["Content-Type: application/json", "x-session-token:"+session_token]
	
	# Create a request node for getting the highscore
	get_name_http = HTTPRequest.new()
	add_child(get_name_http)
	get_name_http.connect("request_completed", self, "_on_player_get_name_request_completed")
	# Send request
	get_name_http.request(url, headers, true, HTTPClient.METHOD_GET, "")
	
func _on_player_get_name_request_completed(result, response_code, headers, body):
	var json = JSON.parse(body.get_string_from_utf8())
	
	emit_signal("name_gotten", json.result.name)
	# Print data
	print(json.result)
	# Print player name
	print(json.result.name)

func _on_upload_score_request_completed(result, response_code, headers, body) :
	var json = JSON.parse(body.get_string_from_utf8())
	
	# Print data
	print(json.result)
	
	# Clear node
	submit_score_http.queue_free()
	emit_signal("score_uploaded")
