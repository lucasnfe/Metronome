using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SynchronizerData;

public class MetronomeLevelGenerator : MisLevelGenerator {

	public int _roomWidth = 5;
	public int _roomHeight = 5;
	public int _roomAmount = 1;

	private BeatObserver _beatObserver;

	struct MetronomeLevel {

		public List<Room> rooms;
		public int maxHeight;
	}

	struct Room {

		public int x, y;
		public int[,] platforms;

		public Room(int x, int y, int width, int height) {

			this.x = x;
			this.y = y;

			platforms = new int[width, height];
		}
	}

	MetronomeLevel _levelData;

	// Use this for initialization
	protected override void Awake () {

		base.Awake ();

		_beatObserver = GetComponent<BeatObserver>();
	}

	void Update() {

		if ((_beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat) {

			PlacePlatform ();
		}

//		if ((_beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat) {
//			
//		}
	}

	protected override void GenerateLevel(float startPosX, float endPosX, float startPosY, float tileSize) {

		_levelData = GenerateRoomGrid(_roomAmount);

		Vector2 startPos = new Vector2 (startPosX, startPosY);
		Vector2 nextRoom = Vector2.zero;

		for (int i = 0; i < _levelData.rooms.Count; i++) {

			Vector2 pos = new  Vector2 (_levelData.rooms[i].x, _levelData.rooms[i].y);

			if(i + 1 < _levelData.rooms.Count)
				nextRoom  = new  Vector2 (_levelData.rooms[i + 1].x, _levelData.rooms[i + 1].y);

			// Build the walls to close the room
			BuildRoomWalls (startPos, tileSize);

			startPos.x += (nextRoom.x - pos.x) * _roomWidth * tileSize;
			startPos.y += (nextRoom.y - pos.y) * _roomHeight * tileSize;
		}
	}

	MetronomeLevel GenerateRoomGrid(int roomAmount) {

		MetronomeLevel level = new MetronomeLevel();
		level.rooms = new List<Room>();

		int lastY = 0;
		int maxHeight = 0;

		level.rooms.Add(new Room(0, 0, _roomWidth, _roomHeight));
	
		for (int i = 1; i <= roomAmount; i++) {

			// Pick a random element in a column
			int currentY = Random.Range(0, MisConstants.LEVEL_MAX_HEIGHT);

			// Flip a coin to decide if the height will change
			if(Random.value > 0.5f)
				currentY = lastY;

			// Connect previous and current rooms
			int vertDist = currentY - lastY;
			for (int j = 1; j <= Mathf.Abs (vertDist); j++) {

				int y = lastY + j * (int)Mathf.Sign (vertDist);
				level.rooms.Add (new Room (i - 1, y, _roomWidth, _roomHeight));
			}

			level.rooms.Add(new Room(i, currentY, _roomWidth, _roomHeight));
			if (currentY > maxHeight)
				maxHeight = currentY;

			lastY = currentY;
		}

		level.maxHeight = maxHeight + 1;
		return level;
	}

	void BuildRoomWalls(Vector2 startPos, float tileSize) {

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		for (int i = -_roomWidth * 2; i < _roomWidth * 2; i++) {

			for (int j = -_roomHeight * 2; j < _roomHeight * 2; j++) {
				
				Vector2 pos1 = startPos + new Vector2 (i, j) * tileSize;

				if(i >= 0 && i < _roomWidth && j == -1)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

				else if(i >= 0 && i < _roomWidth && j == _roomHeight)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

				else if(j >= 0 && j < _roomHeight && i == -1)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);

				else if(j >= 0 && j < _roomHeight && i == _roomWidth)
					BuildCollidableTile (pos1, _level.transform, sprite, _colliderOffset);
				
				else 
					BuildTile (pos1, _level.transform, sprite, _colliderOffset);
			}
		}

		for (int i = 0; i < _roomWidth; i++) {

			for (int j = 0; j < _roomHeight; j++) {

				Vector2 pos1 = startPos + new Vector2 (i, j) * tileSize;
				DestroyTile (pos1, _level.transform, sprite, _colliderOffset);
			}
		}
	}

	public void PlacePlatform() {

		float tileSize = (float)MisConstants.TILE_SIZE /(float) MisConstants.PIXEL_UNIT;

		// Find the room that the player is right now
		Vector2 heroPos = CalcHeroRelativePos (MisGameWorld.Instance.GameHero.transform.position);

		Vector2 roomPos = _startPosition;
		Vector2 nextRoom = Vector2.zero;

		int roomIndex = -1;
		for (int i = 0; i < _levelData.rooms.Count; i++) {
				
			Vector2 pos = new  Vector2 (_levelData.rooms[i].x, _levelData.rooms[i].y);

			if(i + 1 < _levelData.rooms.Count)
				nextRoom  = new  Vector2 (_levelData.rooms[i + 1].x, _levelData.rooms[i + 1].y);

			if (_levelData.rooms[i].x == (int)heroPos.x && 
				_levelData.rooms[i].y == (int)heroPos.y) {

				roomIndex = i;
				break;
			}
				
			roomPos.x += (nextRoom.x - pos.x) * _roomWidth * tileSize;
			roomPos.y += (nextRoom.y - pos.y) * _roomHeight * tileSize;
		}
									
		Vector2 tilePos = PickRandomEmptyTileInRoom(roomIndex);
		_levelData.rooms[roomIndex].platforms [(int)tilePos.x, (int)tilePos.y] = 1;

		Sprite sprite = _surface [Random.Range (0, _surface.Length)];

		Vector2 pos1 = roomPos + new Vector2 (tilePos.x, tilePos.y) * tileSize;
		BuildCollidableTile (pos1, _level.transform, sprite, Vector2.zero);
	}

	private Vector2 CalcHeroRelativePos(Vector2 heroGlobalPos) {

		float tileSize = (float)MisConstants.TILE_SIZE /(float) MisConstants.PIXEL_UNIT;

		Vector2 relativePos = new Vector2 (heroGlobalPos.x, heroGlobalPos.y);
		relativePos.x += tileSize/2f;
		relativePos.y += tileSize/2f;

		relativePos.x = relativePos.x / (float)(_roomWidth * _roomAmount * tileSize);
		relativePos.y = relativePos.y / (float)(_roomHeight * _levelData.maxHeight * tileSize);

		relativePos.x = (int)(relativePos.x * _roomAmount);
		relativePos.y = (int)(relativePos.y * _levelData.maxHeight);

		return relativePos;
	}

	private Vector2 PickRandomEmptyTileInRoom(int roomIndex) {

		int randomX = Random.Range (0, _roomWidth);
		int randomY = Random.Range (0, _roomHeight);

		if (_levelData.rooms[roomIndex].platforms [randomX, randomY] == 1) {

			for (int i = 0; i < _roomWidth; i++) {

				for (int j = 0; j < _roomHeight; j++) {

					if (_levelData.rooms[roomIndex].platforms [i, j] == 0) {

						randomX = i;
						randomY = j;
					}
				}
			}
		}

		return new Vector2 (randomX, randomY);
	}
}
