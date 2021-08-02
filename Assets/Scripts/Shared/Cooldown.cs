using System;
using UnityEngine;

public class Cooldown
{
  float _intervalAnchor = 1f;
  Action _expression;

  public Cooldown (Action expression)
  {
    _expression = expression;
  }

  /// <summary>
  /// Evaluates an expression at specified intervals (in seconds).
  /// Put into the Update function to call every frame.
  /// </summary>
  /// <param name="interval"></param>
  public void Execute ()
  {
    if (_intervalAnchor >= 1f)
    {
      _intervalAnchor = 0f;
      if (_expression != null)
      {
        _expression ();
      }
    }
  }

  /// <summary>
  /// doc later.
  /// </summary>
  /// <param name="interval"></param>
  public void Count (float interval)
  {
    if (_intervalAnchor >= 1f) return;
    _intervalAnchor += Time.deltaTime / interval;
  }
}
