internal class GameEvents
{
    public static string LEVEL_IS_SELECTED_FOR_STARTED { get; private set; } = "LEVEL_IS_SELECTED_FOR_STARTED";


    public static string GAME_STARTED { get; private set; } = "GAME_STARTED";
    public static string LEVEL_RESTART { get; private set; } = "LEVEL_RESTART";
    public static string LEVEL_COMPLETE { get; private set; } = "LEVEL_COMPLETE";
    public static string LEVEL_FAILED { get; private set; } = "LEVEL_FAILED";
    public static string LEVEL_PASS_DATA_COLLECTED { get; private set; } = "LEVEL_PASS_DATA_COLLECTED";
    public static string GAME_ENDING { get; private set; } = "GAME_ENDING";
    public static string LEVEL_EXIT { get; private set; } = "LEVEL_EXIT";

    public static string ON_PAUSE_STATE_CHANGED { get; private set; } = "ON_PAUSE_STATE_CHANGED";


    public static string GAME_INDICATORS_STARTED { get; private set; } = "GAME_INDICATORS_STARTED";


    public static string ITEM_COLLECTED { get; private set; } = "ITEM_COLLECTED";


    public static string DIAMOND_CHANGED { get; private set; } = "DIAMOND_CHANGED";
    public static string PLAYER_HEALTH_CHANGED { get; private set; } = "PLAYER_HEALTH_CHANGED";
    public static string PLAYER_ENERGY_CHANGED { get; private set; } = "PLAYER_ENERGY_CHANGED";
    public static string PLAYER_DAGGER_CHANGED { get; private set; } = "PLAYER_DAGGER_CHANGED";


    public static string ENEMY_KILLED { get; private set; } = "ENEMY_KILLED";
}
