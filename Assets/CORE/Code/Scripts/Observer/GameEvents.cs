internal class GameEvents
{
    public static string ALL_MANAGERS_STARTED { get; private set; } = "ALL_MANAGERS_STARTED";



    public static string LEVEL_IS_SELECTED_FOR_STARTED { get; private set; } = "LEVEL_IS_SELECTED_FOR_STARTED";


    public static string ON_PAUSE_STATE_CHANGED { get; private set; } = "ON_PAUSE_STATE_CHANGED";


    public static string GAME_INDICATORS_STARTED { get; private set; } = "GAME_INDICATORS_STARTED";


    public static string ITEM_COLLECTED { get; private set; } = "ITEM_COLLECTED";


    public static string DIAMOND_CHANGED { get; private set; } = "DIAMOND_CHANGED";
    public static string PLAYER_HEALTH_CHANGED { get; private set; } = "PLAYER_HEALTH_CHANGED";
    public static string PLAYER_ENERGY_CHANGED { get; private set; } = "PLAYER_ENERGY_CHANGED";
    public static string PLAYER_DAGGER_CHANGED { get; private set; } = "PLAYER_DAGGER_CHANGED";
}
