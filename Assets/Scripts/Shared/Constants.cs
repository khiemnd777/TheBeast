public class Constants
{
  public const string EVENT_EMIT_MESSAGE = "emit message";
  public const string EVENT_RECEIVE_EMIT_MESSAGE = "receive emit message";
  public const string EVENT_REGISTER = "register";
  public const string EVENT_CLONE_EVERYWHERE = "clone everywhere";
  public const string EVENT_BROADCAST_CLONE_EVERYWHERE = "broadcast clone everywhere";
  public const string EVENT_OBJECT_REGISTER = "object register";
  public const string EVENT_OBJECT_REGISTERED = "object registered";
  public const string EVENT_OTHER_OBJECT_REGISTERED = "other object registered";
  public const string EVENT_CONNECT = "connect";
  public const string EVENT_OBJECT_TRANSFORM = "object transform";
  public const string EVENT_SERVER_OBJECT_TRANSFORM = "object transform";
  public const string EVENT_SERVER_REGISTER = "server register";
  public const string EVENT_SERVER_REGISTER_FINISHED = "server register finished";
  public const string EVENT_CLIENT_REGISTER_FINISHED = "client register finished";
  public const string EVENT_REQUEST_GETTING_PLAYERS = "request getting players";

  #region Player
  public const string EVENT_REQUIRE_REGISTER_PLAYER = "server register player";
  public const string EVENT_SERVER_CONNECT = "connect";
  public const string EVENT_SERVER_PLAYER_TRANSLATE = "player translate";
  public const string EVENT_SERVER_PLAYER_ROTATE = "player rotate";
  public const string EVENT_SERVER_LOAD_PLAYERS = "load players";
  public const string EVENT_SERVER_WEAPON_TRIGGER = "weapon trigger";
  public const string EVENT_PLAYER_STORE_ID = "player store id";
  public const string EVENT_CLIENT_LOADED_PLAYER = "loaded player";
  public const string EVENT_CLIENT_PLAYER_TRANSLATE = "player translate";
  public const string EVENT_CLIENT_PLAYER_ROTATE = "player rotate";
  public const string EVENT_CLIENT_PLAYER_DEAD = "player dead";
  public const string EVENT_CLIENT_PLAYER_SYNC_HP = "player sync hp";
  public const string EVENT_CLIENT_PLAYER_SYNC_MAX_HP = "player sync max hp";
  public const string EVENT_CLIENT_EMPTY_LIST = "empty list";
  public const string EVENT_CLIENT_CONNECTED = "connected";
  public const string EVENT_CLIENT_REGISTERED = "registered";
  public const string EVENT_CLIENT_OTHER_REGISTERED = "other registered";
  public const string EVENT_CLIENT_OTHER_DISCONNECTED = "other disconnected";
  public const string EVENT_CLIENT_REGISTER_PLAYER_FINISHED = "register player finished";
  public const string EVENT_REQUIRE_GETTING_PLAYERS = "request getting players";
  public const string EVENT_RESPONSE_GETTING_PLAYERS = "response getting players";
  public const string EVENT_DOWNLOAD_PLAYERS = "download players";

  #endregion

  #region Bullet
  public const string EVENT_SERVER_BULLET_REMOVE = "bullet remove";
  #endregion
}
