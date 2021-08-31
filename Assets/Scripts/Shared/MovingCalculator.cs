using UnityEngine;

public class MovingCalculator
{
  public event System.Action onMoving;
  Point _direction;
  Point _point;

  /// <summary>
  /// The current position.
  /// </summary>
  /// <value></value>
  public Point point
  {
    get
    {
      return new Point(_point.x, _point.y);
    }
  }

  /// <summary>
  /// The current direction.
  /// </summary>
  /// <value></value>
  public Point direction
  {
    get
    {
      return new Point(_direction.x, _direction.y);
    }
  }

  public MovingCalculator(Point initPoint)
  {
    _point = new Point(initPoint.x, initPoint.y);
    _direction = new Point();
  }

  /// <summary>
  /// Calculate the current position and direction through x-axis and y-axis
  /// </summary>
  /// <param name="horizontal"></param>
  /// <param name="vertical"></param>
  public void Calculate(float horizontal, float vertical)
  {
    _point = new Point(horizontal, vertical);
    _direction = _point.normalize;
    if (_point.magnitude > 0f)
    {
      if (onMoving != null)
      {
        onMoving();
      }
    }
  }

  /// <summary>
  /// Calculate the current position and direction through x-axis and y-axis
  /// </summary>
  /// <param name="newPoint"></param>
  public void Calculate(Point newPoint)
  {
    Calculate(newPoint.x, newPoint.y);
  }
}
