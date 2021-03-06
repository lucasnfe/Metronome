﻿using UnityEngine;
using System.Collections;

public enum ENEMIES {

	MINION,
	BOSS
}

public enum PLATFORMS {

	STATIC1,
	STATIC2,
	BREAKABLE,
	EVENT,
	TETRIS1,
	TETRIS2,
	TETRIS3,
	TETRIS4,
	TETRIS5
}

public enum CHARACTER_SFX {

	RUN,
	JUMP,
	SHOOT,
}

public enum DESTROY_SFX {

	HIT,
	DESTROY,
	CREATE
}
	
public class MisConstants {

	// PHYSICS SYSTEM CONSTANTS
	public static readonly float WALL_FRICTION	      = 0.25f;
	public static readonly float AIR_FRICTION	      = 0.4f;
	public static readonly float PLAYER_SKIN          = 0.0005f;
	public static readonly float MAX_SPEED     		  = 1f;

	// TILE_SIZE = A power of two number -1f in order the avoid placement bugs
	public static readonly float   PIXEL_UNIT     	  = 100f;
	public static readonly float   TILE_SIZE      	  = 16f/PIXEL_UNIT;

	// LEVEL GENERATOR CONSTANTS
	public static readonly int   LEVEL_GROUND_HEIGHT  = 3;
	public static readonly int   LEVEL_HEIGHT  = 12;

	// EVENT PLATFORM TAGS
	public static readonly string TAG_WALL 		      = "Wall";

	public static readonly int[][,] PLATFORM_DESCRIPTOR = { 
		new int [,] 
		{{0, 0, 0}, 
		 {0, 1, 0}, 
		 {0, 0, 0}},
		new int [,] 
		{{0, 0, 0}, 
		 {0, 1, 0}, 
		 {0, 0, 0}},
		new int [,] 
		{{0, 0, 0},
		 {0, 1, 0},
		 {0, 0, 0}},
		new int [,] 
		{{1, 0, 0},
		 {1, 0, 0},
		 {1, 0, 0}},
		new int [,] 
		{{0, 0, 0},
		 {0, 1, 0},
		 {1, 1, 1}},
		new int [,] 
		{{1, 1, 1},
		 {1, 0, 0},
		 {0, 0, 0}},
		new int [,] 
		{{1, 1, 0},
		 {1, 1, 0},
		 {0, 0, 0}},
		new int [,] 
		{{1, 0, 0},
		 {1, 1, 0},
		 {0, 1, 0}}
	};
}
