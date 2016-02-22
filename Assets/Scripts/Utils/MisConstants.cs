using UnityEngine;
using System.Collections;

public class MisConstants {

	// PHYSICS SYSTEM CONSTANTS
	public static readonly float WALL_FRICTION	      = 0.15f;
	public static readonly float AIR_FRICTION	      = 0.4f;
	public static readonly float PLAYER_SKIN          = 0.0005f;
	public static readonly float SAFETY_GAP			  = 0.005f;
	public static readonly float MAX_SPEED     		  = 1f;

	// TILE_SIZE = A power of two number -1f in order the avoid placement bugs
	public static readonly int   TILE_SIZE      	  = 64 - 1;
	public static readonly int   PIXEL_UNIT     	  = 100;
	
	// LEVEL GENERATOR CONSTANTS
	public static readonly int   MAX_GAP_SIZE 		  = 2;
	public static readonly float NON_ENEMIES_AREA 	  = 4f;
	public static readonly int   LEVEL_MAX_WIDTH 	  = 12;
	public static readonly int   LEVEL_MAX_HEIGHT 	  = 5;

	// EVENT PLATFORM TAGS
	public static readonly string TAG_KILLZONE 		  = "Killzone";
	public static readonly string TAG_WALL 		      = "Wall";

	public enum ENEMIES {

		ALIEN
	}

	public enum PLATFORMS {

		STATIC,
		BREAKABLE,
	}
}
