using System;

namespace Net.Socket
{
  public interface ISocketWrapper
  {
    /// <summary>
    /// Register socket message
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="callback"></param>
    void On (string evt, Action<SocketEvent> callback);

    /// <summary>
    /// Emit message to server
    /// </summary>
    /// <param name="evt"></param>
    void Emit (string evt);

    /// <summary>
    /// Emit message to the server-side.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="data"></param>
    void Emit (string evt, object data);

    /// <summary>
    /// Emit message to server
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="json"></param>
    void Emit (string evt, string json);
  }
}
