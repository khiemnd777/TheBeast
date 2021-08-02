namespace Net.Socket
{
  public class SocketEvent
  {
    /// <summary>
    /// Event name
    /// </summary>
    /// <value></value>
    public string name { get; set; }

    /// <summary>
    /// Event data
    /// </summary>
    /// <value></value>
    public object data { get; set; }

    public SocketEvent (string name) : this (name, null)
    {
      this.name = name;
    }

    public SocketEvent (string name, object data)
    {
      this.name = name;
      this.data = data;
    }
  }
}
