using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SynchronizerData;

public class MetronomeLevelGenerator : MisLevelGenerator {

	public int _roomWidth  = 5;
	public int _roomHeight = 5;

	public int _bossRoomScale = 1;

	private int _roomAmount = 1;	

	private BeatObserver _beatObserver;
	private BeatSynchronizer _beatSync;

	struct MetronomeLevel {

		public List<Room> rooms;
		public int maxHeight;
	}

	struct Room {

		public int x, y;
		public int width, height;
		public int[,] platforms;

		public Room(int x, int y, int width, int height) {

			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;

			platforms = new int[width, height];
		}
	}

	MetronomeLevel _levelData;

	// Use this for initialization
	protected override void Awake () {

		base.Awake ();

		_beatObserver = GetComponent<BeatObserver>();
		_beatSync = GetComponent<BeatSynchronizer>();

		_roomAmount = (int)_beatSync.bpm/4;
	}

	void Update() {

		if (!MisGameWorld.Instance.GameHero)
			return;

		if ((_beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat) {

//			PlacePlatform ();
		}

		if ((_beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat) {

//			PlaceEnemies ();
		}
	}

	protected override void GenerateLevel(float startPosX, float startPosY, float tileSize) {

		_levelData = GenerateRoomGrid(_roomAmount);

		Vector2 startPos = new Vector2 (startPosX, startPosY);
		Vector2 nextRoom = Vector2.zero;

		for (int i = 0; i < _levelData.rooms.Count; i++) {

			Vector2 pos = new  Vector2 (_levelData.rooms[i].x, _levelData.rooms[i].y);

			if(i + 1 < _levelData.rooms.Count)
				nextRoom  = new  Vector2 (_levelData.rooms[i + 1].x, _levelData.rooms[i + 1].y);

			// Build the walls to close the room
			BuildRoomWalls (_levelData.rooms[i], startPos, tileSize);

			startPos.x += (nextRoom.x - pos.x) * _levelData.rooms[i].width * tileSize;
			startPos.y += (nextRoom.y - pos.y) * _levelData.rooms[i].height * tileSize;
		}
	}

	MetronomeLevel GenerateRoomGrid(int roomAmount) {

		MetronomeLevel level = new MetronomeLevel();
		level.rooms = new List<Room>();

		int lastY = 0;
		int lastWidth  = 0;
		int lastHeight = 0;
		int maxHeight  = 0;

		level.rooms.Add(new Room(0, 0, _roomWidth, _roomHeight));
	
		for (int i = 1; i <= roomAmount; i++) {

			int width = _roomWidth;
			int height = _roomHeight;

			// Pick a random element in a column
			int currentY = Random.Range (0, MisConstants.LEVEL_MAX_HEIGHT);

			// Boss room
			if (i == roomAmount / 2 || i == roomAmount) {
				
				width *= _bossRoomScale;
				height *= _bossRoomScale;

				currentY = lastY;
				level.rooms.Add (new Room (i, lastY, width, height));

			} else {
				
				// Flip a coin to decide if the height will change
				if (Random.value > 0.5f)
					currentY = lastY;

				// Connect previous and current rooms
				int vertDist = currentY - lastY;
				for (int j = 0; j <= Mathf.Abs (vertDist); j++) {

					int y = lastY + j * (int)Mathf.Sign (vertDist);
					level.rooms.Add (new Room (i - 1, y, lastWidth, lastHeight));
				}
				
				width *= Random.Range (1, _bossRoomScale);
				height *= Random.Range (1, _bossRoomScale);

				level.rooms.Add (new Room (i, currentY, width, height));
				if (currentY > maxHeight)
					maxHeight = currentY;
			}

			lastY = currentY;
			lastWidth = width;
			lastHeight = width;
		}

		level.maxHeight = maxHeight + 1;
		return level;
	}


	void BuildRoomWalls(Room room, Vector2 startPos, float tileSize) {

		GameObject staticPlat = _platforms [(int)PLATFORMS.STATIC];

		for (int i = -room.width * 2; i < room.width * 2; i++) {

			for (int j = -room.height * 2; j < room.height * 2; j++) {

				Vector2 pos1 = startPos + new Vector2 (i, j) * tileSize;
				BuildTile (pos1, _level.transform, staticPlat);
			}
		}

		for (int i = 0; i < room.width; i++) {
			
			for (int j = 0; j < room.height; j++) {

				DestroyTile (startPos + new Vector2 (i, j) * tileSize);
			}
		}

	}

	public void PlacePlatform() {

		GameObject platform = _platforms [(int)PLATFORMS.BREAKABLE];
		Vector2 pos1 = PickRandomTileInCurrentRoom ();
		BuildTile (pos1, _level.transform, platform, true);
	}

	public void PlaceEnemies() {

		GameObject platform = _enemies [(int)ENEMIES.BAT];
		Vector2 pos1 = PickRandomTileInCurrentRoom ();
		BuildTile (pos1, _level.transform, platform, true);
	}

	private Vector2 PickRandomTileInCurrentRoom() {

		float tileSize = (float)MisConstants.TILE_SIZE /(float) MisConstants.PIXEL_UNIT;

		// Find the room that the player is right now
		Vector2 heroPos = CalcHeroRelativePos (MisGameWorld.Instance.GameHero.transform.position);

		Vector2 roomPos = _startPosition;
		int roomIndex = -1;

		FindCurrentRoom (heroPos, tileSize, ref roomIndex, ref roomPos);

		Vector2 tilePos = PickRandomEmptyTileInRoom(roomIndex);
		_levelData.rooms[roomIndex].platforms [(int)tilePos.x, (int)tilePos.y] = 1;
		Vector2 randomPos = roomPos + new Vector2 (tilePos.x, tilePos.y) * tileSize;

		return randomPos;
	}

	private void FindCurrentRoom(Vector2 heroPos, float tileSize, ref int roomIndex, ref Vector2 roomPos) {

		roomIndex = -1;
		roomPos   = _startPosition;

		Vector2 nextRoom = Vector2.zero;

		for (int i = 0; i < _levelData.rooms.Count; i++) {

			Vector2 pos = new  Vector2 (_levelData.rooms[i].x, _levelData.rooms[i].y);

			if(i + 1 < _levelData.rooms.Count)
				nextRoom  = new  Vector2 (_levelData.rooms[i + 1].x, _levelData.rooms[i + 1].y);

			if (_levelData.rooms[i].x == (int)heroPos.x && 
				_levelData.rooms[i].y == (int)heroPos.y) {

				roomIndex = i;
				break;
			}

			roomPos.x += (nextRoom.x - pos.x) * _levelData.rooms[i].width * tileSize;
			roomPos.y += (nextRoom.y - pos.y) * _levelData.rooms[i].height * tileSize;
		}
	}

	private Vector2 CalcHeroRelativePos(Vector2 heroGlobalPos) {

		float tileSize = (float)MisConstants.TILE_SIZE /(float) MisConstants.PIXEL_UNIT;

		Vector2 relativePos = new Vector2 (heroGlobalPos.x, heroGlobalPos.y);
		relativePos.x += tileSize/2f;
		relativePos.y += tileSize/2f;

		relativePos.x = relativePos.x / (float)(_roomWidth  * _roomAmount * tileSize);
		relativePos.y = relativePos.y / (float)(_roomHeight * _levelData.maxHeight * tileSize);

		relativePos.x = (int)(relativePos.x * _roomAmount);
		relativePos.y = (int)(relativePos.y * _levelData.maxHeight);

		return relativePos;
	}

	private Vector2 PickRandomEmptyTileInRoom(int roomIndex) {

		int randomX = Random.Range (0, _levelData.rooms[roomIndex].width);
		int randomY = Random.Range (0, _levelData.rooms[roomIndex].height);

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
