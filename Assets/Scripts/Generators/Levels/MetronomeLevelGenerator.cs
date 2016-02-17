using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetronomeLevelGenerator : MisLevelGenerator {

	public int roomWidth;
	public int roomHeight;

	private Vector2 _nextRoom;

	struct Room {

		public int x, y;

		public Room(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	List<Room> _levelData;

	// Use this for initialization
	protected override void Awake () {

		base.Awake ();
	}

	protected override void GenerateLevel(float startPosX, float endPosX, float startPosY, float tileSize) {

		_levelData = GenerateRoomGrid(30);

		Vector2 startPos = new Vector2(startPosX, startPosY);

		for (int i = 0; i < _levelData.Count; i++) {

			Vector2 pos = new  Vector2 (_levelData[i].x, _levelData[i].y);

			if(i + 1 < _levelData.Count)
				_nextRoom  = new  Vector2 (_levelData[i + 1].x, _levelData[i + 1].y);

			// Build the walls to close the room
			BuildRoomWalls (startPos, tileSize);

			startPos.x += (_nextRoom.x - pos.x) * roomWidth * tileSize;
			startPos.y += (_nextRoom.y - pos.y) * roomHeight * tileSize;
		}
	}

	List<Room> GenerateRoomGrid(int roomAmount) {

		List<Room> roomGrid = new List<Room>();

		int lastY = 0;

		for (int i = 0; i < roomAmount; i++) {

			// Pick a random element in a column
			int side = Random.value > 0.5f ? 1 : -1;
			int currentY = lastY + Random.Range(0, MisConstants.LEVEL_MAX_HEIGHT) * side;

			// Connect previous and current rooms
			if (i > 0) {

				if(Random.value > 0.5f)
					currentY = lastY;
				
				int vertDist = currentY - lastY;
				for (int j = 1; j <= Mathf.Abs (vertDist); j++) {

					Room room = new Room (i - 1, lastY + j * (int)Mathf.Sign (vertDist));
					roomGrid.Add (room);
				}
			}

			roomGrid.Add(new Room(i, currentY));

			lastY = currentY;
		}

		return roomGrid;
	}

	void BuildRoomWalls(Vector2 startPos, float tileSize) {

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		for (int i = -roomWidth * 2; i < roomWidth * 2; i++) {

			for (int j = -roomHeight * 2; j < roomHeight * 2; j++) {
				
				Vector2 pos1 = startPos + new Vector2 (i, j) * tileSize;

				if(i >= 0 && i < roomWidth && j == -1)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

				else if(i >= 0 && i < roomWidth && j == roomHeight)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

				else if(j >= 0 && j < roomHeight && i == -1)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

				else if(j >= 0 && j < roomHeight && i == roomWidth)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);
				
				else 
					BuildTile (pos1, _level.transform, sprite, _colliderOffset);
			}
		}

		for (int i = 0; i < roomWidth; i++) {

			for (int j = 0; j < roomHeight; j++) {

				Vector2 pos1 = startPos + new Vector2 (i, j) * tileSize;
				DestroyTile (pos1, _level.transform, sprite, _colliderOffset);
			}
		}
	}
}
