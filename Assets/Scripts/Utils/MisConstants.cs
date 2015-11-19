using UnityEngine;
using System.Collections;

public class MisConstants {

	// PHYSICS SYSTEM CONSTANTS
	public static readonly float GRAVITY              = Physics2D.gravity.y/20f;
	public static readonly float PLAYER_SKIN          = 0.0005f;
	public static readonly float MAX_SPEED     		  = 25f;
	public static readonly float FRICTION_COEF_AIR    = 0.6f;
	public static readonly float FRICTION_COEF_GROUND = 0.6f;
	public static readonly int   COLLISION_RAYS 	  = 3;

	// TILE_SIZE = A power of two number -1f in order the avoid placement bugs
	public static readonly float TILE_SIZE     		  = 64f - 1f;
	
	// LEVEL GENERATOR CONSTANTS
	public static readonly int MAX_GAP_SIZE 		  = 2;

	// EVENT PLATFORM TAGS
	public static readonly string TAG_KILLZONE 		  = "Killzone";

	// CHARACTER NAMES
	public static readonly string HERO_NAME 		  = "MisHero";
}
