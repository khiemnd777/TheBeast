using UnityEngine;

/// <summary>
/// Creates a new vector with given x, y axis.
/// </summary>
[System.Serializable]
public struct Point
{
  public float x;
  public float y;

  public Point(float x, float y)
  {
    this.x = x;
    this.y = y;
  }

  public Point normalize
  {
    get
    {
      return Normalize(this);
    }
  }

  public float magnitude
  {
    get
    {
      return Magnitude(this);
    }
  }

  public void Normalize()
  {
    this = Normalize(this);
  }

  public static Point FromVector3(Vector3 fromVector3)
  {
    return new Point(fromVector3.x, fromVector3.z);
  }

  public static Point FromArray(float[] position)
  {
    return new Point(position[0], position[1]);
  }

  public static Vector3 ToVector3(Vector3 toVector3, Point point)
  {
    return new Vector3(point.x, toVector3.y, point.y);
  }

  public static float Magnitude(Point point)
  {
    var length = Mathf.Sqrt(point.x * point.x + point.y * point.y);
    return length;
  }

  public static Point Normalize(Point point)
  {
    var magnitude = Magnitude(point);
    return magnitude == 0 ? new Point(0, 0) : new Point(point.x / magnitude, point.y / magnitude);
  }

  public static Point operator +(Point point1, Point point2)
  {
    return new Point(point1.x + point2.x, point1.y + point2.y);
  }

  public static Point operator -(Point point1, Point point2)
  {
    return new Point(point1.x - point2.x, point1.y - point2.y);
  }

  public static Point operator *(Point point, float number)
  {
    return new Point(point.x * number, point.y * number);
  }

  public static Point operator /(Point point, float number)
  {
    return new Point(point.x / number, point.y / number);
  }
}
