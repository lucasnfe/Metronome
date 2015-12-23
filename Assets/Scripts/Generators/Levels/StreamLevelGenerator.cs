using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StreamLevelGenerator : MisLevelGenerator {

	public Vector2 roomSize;
	private Vector2 _lastRoomDir;

	struct Room {

		public Vector2 pos;
		public List<Vector2> dirs;

		public Room(Vector2 pos, List<Vector2> dirs) {
			this.pos = pos;
			this.dirs = dirs;
		}
	}

	List<List<Room>> _levelData;

	JSONObject _jsonFile;

	// Use this for initialization
	 void Awake () {

		_levelData = new List<List<Room>> ();

		// Parse stream level
		TextAsset file = Resources.Load ("StreamLevels/level1") as TextAsset;
		_jsonFile = new JSONObject(file.text);
	}
	
	void ParseStreamLevel() {

		for(int i = 0; i < _jsonFile["level"].list.Count; i++) {

			List<Room> path = new List<Room>();
			JSONObject pathData = _jsonFile["level"].list[i]["path"];

			for(int j = 0; j < pathData.list.Count; j++) {

				int pi1 = pathData.keys[j].IndexOf('-');
				int pi2 = pathData.keys[j].IndexOf('x');

				Vector2 roomPos;
				roomPos.x = int.Parse(pathData.keys[j].Substring(pi1 + 1, pi2 - pi1 - 1));
				roomPos.y = int.Parse(pathData.keys[j].Substring(pi2 + 1));

				List<Vector2> dirs = new List<Vector2>();
				JSONObject roomData = pathData.list[j];
				
				for(int k = 0; k < roomData.list.Count; k++) {
			
					float x = float.Parse(roomData.list[k]["x"].ToString());
					float y = float.Parse(roomData.list[k]["y"].ToString());

					dirs.Add (new Vector2(x, y));
				}

				Room room = new Room(roomPos, dirs);
				path.Add (room);
			}

			_levelData.Add (path);
		}
	}

	protected override void GenerateLevel(float startPosX, float endPosX, float startPosY, float tileSize) {
		
		if (_jsonFile)
			ParseStreamLevel ();
		
		for (int i = 0; i < _levelData.Count; i++) {

			Vector2 startPos = new Vector2(startPosX, startPosY);
			Vector2 nextRoomIndex = Vector2.zero;

			for (int j = 0; j < _levelData[i].Count; j++) {

				if(j + 1 < _levelData[i].Count)
					nextRoomIndex = _levelData[i][j + 1].pos;
								
				// Build the walls to close the room
				BuildRoomWalls(startPos, roomSize, tileSize, nextRoomIndex - _levelData[i][j].pos);

				// Generate the platform challegens using the data points
				GenerateRoom(startPos, roomSize, tileSize, nextRoomIndex - _levelData[i][j].pos, _levelData[i][j]);

				startPos += _lastRoomDir * roomSize.x * tileSize;
			}
		}
	}

	void GenerateRoom(Vector2 startPos, Vector2 size, float tileSize, Vector2 nextDir, Room room) {

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		Vector2 averagePoint = Vector2.zero;

		for (int i = 0; i < room.dirs.Count; i++)
			averagePoint += room.dirs[i];

		averagePoint = averagePoint / room.dirs.Count;
		averagePoint = new Vector2 (float.Parse (averagePoint.x.ToString ("0.0")) * 10f,
		                            float.Parse (averagePoint.y.ToString ("0.0")) * 10f);

		Vector2 inverseAveragePoint = size - averagePoint;

		float dist = Vector2.Distance (averagePoint, inverseAveragePoint);
		float minPos = Mathf.Min (averagePoint.x, inverseAveragePoint.x);

		float coin = Random.value > 0.5f ? 1f : -1f;
		float randOffset = Random.Range (0, (int)dist) * coin;

		for (int i = 0; i < (int)dist; i++) {

			Vector2 tilePos = new Vector2 ((minPos + i) * tileSize, averagePoint.y * tileSize);

			if(Random.value < 0.95f)
				BuildCollidableTile (startPos + tilePos, _level.transform, sprite, _colliderOffset);

			if(averagePoint.y > 2f) {

				Vector2 jumpSupportTilePos = new Vector2 (tilePos.x + randOffset * tileSize, inverseAveragePoint.y * tileSize);
				if(Random.value < 0.95f)
					BuildCollidableTile (startPos + jumpSupportTilePos, _level.transform, sprite, _colliderOffset);
			}
		}

		if (Mathf.Abs(nextDir.y) == 1f) {

			int halfHeight =  (int)(roomSize.y/2f);
			int randomWidth = (int)((Random.value > 0.5f) ? 0 : roomSize.x - 2);
			Vector2 pos = new Vector2 (randomWidth, halfHeight) * tileSize;

			BuildCollidableTile (startPos + pos, _level.transform, sprite, _colliderOffset);
		}
	}

	void BuildRoomWalls(Vector2 startPos, Vector2 size, float tileSize, Vector2 nextDir) {

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		// Build horizontal walls
		for (float i = startPos.x; i < startPos.x + size.x * tileSize; i += tileSize) {

			Vector2 pos1 = new Vector2 (i, startPos.y);
			Vector2 pos2 = new Vector2 (i, startPos.y + size.y * tileSize);

			if(nextDir.y != -1f && _lastRoomDir.y != 1f)
				BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

			if(nextDir.y != 1f && _lastRoomDir.y != -1f)
				BuildCollidableTile (pos2, _level.transform, sprite, _colliderOffset);
		}
	
		// Build veritcal walls
		for (float i = startPos.y; i < startPos.y + size.y * tileSize; i += tileSize) {

			Vector2 pos1 = new Vector2 (startPos.x, i);
			Vector2 pos2 = new Vector2 (startPos.x + size.x * tileSize, i);

			if(nextDir.x != -1f && _lastRoomDir.x != 1f)
				BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

			if(nextDir.x != 1f && _lastRoomDir.x != -1f)
				BuildCollidableTile (pos2, _level.transform, sprite, _colliderOffset);
		}

		_lastRoomDir = nextDir;
	}
}
