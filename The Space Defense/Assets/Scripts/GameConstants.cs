using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour {

    // projectile characteristics
    public const float BEAM_PROJECTILE_INITIAL_FORCE = 10f;
    public const int ROBOT_BEAM_PROJECTILE_DAMAGE = 5;
    public const float RIFLE_BEAM_PROJECTILE_OFFSET_Y = -0.02f;
    public const float RIFLE_BEAM_PROJECTILE_OFFSET_X = 0.12f;

    public const float ROBOT_PROJECTILE_INITIAL_FORCE = 7.5f;
    public const int FRENCH_FRIES_PROJECTILE_DAMAGE = 5;
    public const float ROBOT_PROJECTILE_OFFSET_X = 0.17f;
    public const float ROBOT_PROJECTILE_OFFSET_Y = -0.05f;

    // robot characteristics
    public const int TOTAL_ROBOTS = 20;
    public const int MAX_ROBOTS = 4;
    public const int ROBOT_POINTS = 10;
    public const int ROBOT_DAMAGE = 10;
    public const float ROBOT_MAX_INITIAL_FORCE = 0.1f;
    public const float ROBOT_MIN_FIRING_DELAY = 0.5f;
    public const float ROBOT_FIRING_RATE_RANGE = 1f;

    public const float ROBOT_MIN_FACING_DELAY = 1.2f;
    public const float ROBOT_FACING_DELAY_RANGE = 1.5F;

    // burger characteristics
    public const float BURGER_MOVEMENT_AMOUNT = 0.1f;
    public const float BURGER_COOLDOWN_SECONDS = 0.5f;
    public const int PLAYER_HEALTH = 100;

    // text display support
    public const string SCORE_PREFIX = "Score: ";
    public const string HEALTH_PREFIX = "Health: ";

    // spawn location support
    public const int SPAWN_BORDER_SIZE = 100;
}
