using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetronomeLevelGenerator : MisLevelGenerator {

	public int roomWidth;
	public int roomHeight;
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

	// Use this for initialization
	protected override void Awake () {

		base.Awake ();

		_levelData = new List<List<Room>> ();
	}

	protected override void GenerateLevel(float startPosX, float endPosX, float startPosY, float tileSize) {
		
		for (int i = 0; i < _levelData.Count; i++) {

			Vector2 startPos = new Vector2(startPosX, startPosY);
			Vector2 nextRoomIndex = Vector2.zero;

			for (int j = 0; j < _levelData[i].Count; j++) {

				if(j + 1 < _levelData[i].Count)
					nextRoomIndex = _levelData[i][j + 1].pos;
								
				// Build the walls to close the room
				BuildRoomWalls(startPos, tileSize, nextRoomIndex - _levelData[i][j].pos);

				// Generate the platform challegens using the data points
				 GenerateRoom(startPos, tileSize, nextRoomIndex - _levelData[i][j].pos, _levelData[i][j]);

				startPos.x += _lastRoomDir.x * roomWidth * tileSize;
				startPos.y += _lastRoomDir.y * roomHeight * tileSize;
			}
		}
	}

	void GenerateRoom(Vector2 startPos, float tileSize, Vector2 nextDir, Room room) {

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		Vector2 averagePoint = Vector2.zero;

		for (int i = 0; i < room.dirs.Count; i++)
			averagePoint += room.dirs[i];

		averagePoint = averagePoint / room.dirs.Count;
		averagePoint = new Vector2 (float.Parse (averagePoint.x.ToString ("0.0")) * 10f,
		                            float.Parse (averagePoint.y.ToString ("0.0")) * 10f);

		Vector2 inverseAveragePoint = new Vector2(roomWidth, roomHeight) - averagePoint;

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

			int halfHeight =  (int)(roomHeight/2f);
			int randomWidth = (int)((Random.value > 0.5f) ? 0 : roomWidth - 2);
			Vector2 pos = new Vector2 (randomWidth, halfHeight) * tileSize;

			BuildCollidableTile (startPos + pos, _level.transform, sprite, _colliderOffset);
		}
	}

	void BuildRoomWalls(Vector2 startPos, float tileSize, Vector2 nextDir) {

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		// Build horizontal walls
		for (int i = 0; i < roomWidth; i++) {

			Vector2 pos1 = startPos + new Vector2 (i, 0) * tileSize;
			Vector2 pos2 = startPos + new Vector2 (i, roomHeight) * tileSize;

			if(nextDir.y != -1f && _lastRoomDir.y != 1f)
				BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

			if(nextDir.y != 1f && _lastRoomDir.y != -1f)
				BuildCollidableTile (pos2, _level.transform, sprite, _colliderOffset);
		}
	
		// Build veritcal walls
		for (int i = 0; i < roomHeight; i++) {

			Vector2 pos1 = startPos + new Vector2 (0, i) * tileSize;
			Vector2 pos2 = startPos + new Vector2 (roomWidth - 1, i) * tileSize;

			if(nextDir.x != -1f && _lastRoomDir.x != 1f)
				BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

			if(nextDir.x != 1f && _lastRoomDir.x != -1f)
				BuildCollidableTile (pos2, _level.transform, sprite, _colliderOffset);
		}

		_lastRoomDir = nextDir;
	}
}
