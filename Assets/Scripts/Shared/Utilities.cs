using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utility
{
  public static Vector3 CameraInBound(Camera camera, BoxCollider bound, Vector3 position)
  {
    return CameraInBound(camera, bound.bounds.min, bound.bounds.max, position);
  }

  public static Vector3 CameraInBound(Camera camera, Vector3 center, float width, float height, Vector3 position)
  {
    var min = new Vector3(center.x - width / 2, 0f, center.z - height / 2);
    var max = new Vector3(center.x + width / 2, 0f, center.z + height / 2);
    return CameraInBound(camera, min, max, position);
  }

  public static Vector3 CameraInBound(Camera camera, Vector3 min, Vector3 max, Vector3 position)
  {
    var halfHeight = camera.orthographicSize;
    var halfWidth = halfHeight * Screen.width / Screen.height;
    var x = Mathf.Clamp(position.x, min.x + halfWidth, max.x - halfWidth);
    var z = Mathf.Clamp(position.z, min.z + halfHeight, max.z - halfHeight);
    return new Vector3(x, position.y, z);
  }

  public static Point LineLineIntersection(Point a, Point b, Point c, Point d, out float determinant)
  {
    // Line AB represented as a1x + b1y = c1 
    var a1 = b.y - a.y;
    var b1 = a.x - b.x;
    var c1 = a1 * (a.x) + b1 * (a.y);

    // Line CD represented as a2x + b2y = c2 
    var a2 = d.y - c.y;
    var b2 = c.x - d.x;
    var c2 = a2 * (c.x) + b2 * (c.y);

    determinant = a1 * b2 - a2 * b1;
    if (determinant == 0)
    {
      // The lines are parallel. This is simplified 
      // by returning a pair of FLT_MAX 
      return new Point(float.MaxValue, float.MaxValue);
    }
    else
    {
      var x = (b2 * c1 - b1 * c2) / determinant;
      var y = (a1 * c2 - a2 * c1) / determinant;
      return new Point(x, y);
    }
  }

  public static Point LineLineIntersection(Camera camera, Vector3 center, Point c, Point d)
  {
    var halfHeight = camera.orthographicSize;
    var halfWidth = halfHeight * Screen.width / Screen.height;
    var a1 = new List<Point>
    {
      new Point(center.x - halfWidth, center.z - halfHeight),
      new Point(center.x + halfWidth, center.z - halfHeight),
      new Point(center.x - halfWidth, center.z + halfHeight),
      new Point(center.x - halfWidth, center.z + halfHeight)
    };
    var a2 = new List<Point>
    {
      new Point(center.x + halfWidth, center.z - halfHeight),
      new Point(center.x + halfWidth, center.z + halfHeight),
      new Point(center.x + halfWidth, center.z + halfHeight),
      new Point(center.x - halfWidth, center.z - halfHeight),
    };
    for (var i = 0; i < a1.Count; i++)
    {
      var intersection = LineLineIntersection(a1[i], a2[i], c, d, out float determinant);
      if (
        AreLineSegmentsIntersectingDotProduct
        (
          new Vector3(a1[i].x, 0f, a1[i].y),
          new Vector3(a2[i].x, 0f, a2[i].y),
          new Vector3(c.x, 0f, c.y),
          new Vector3(d.x, 0f, d.y)
        )
      )
      {
        return intersection;
      }
    }
    return new Point(float.MaxValue, float.MaxValue);
  }

  //Line segment-line segment intersection in 2d space by using the dot product
  //p1 and p2 belongs to line 1, and p3 and p4 belongs to line 2 
  public static bool AreLineSegmentsIntersectingDotProduct(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
  {
    bool isIntersecting = false;

    if (IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2))
    {
      isIntersecting = true;
    }
    return isIntersecting;
  }

  //Are the points on different sides of a line?
  private static bool IsPointsOnDifferentSides(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
  {
    bool isOnDifferentSides = false;

    //The direction of the line
    Vector3 lineDir = p2 - p1;

    //The normal to a line is just flipping x and z and making z negative
    Vector3 lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);

    //Now we need to take the dot product between the normal and the points on the other line
    float dot1 = Vector3.Dot(lineNormal, p3 - p1);
    float dot2 = Vector3.Dot(lineNormal, p4 - p1);

    //If you multiply them and get a negative value then p3 and p4 are on different sides of the line
    if (dot1 * dot2 < 0f)
    {
      isOnDifferentSides = true;
    }

    return isOnDifferentSides;
  }

  public static Vector3 AlterVector3(Vector3 owner, float x, float y)
  {
    return new Vector3(x, owner.y, y);
  }

  public static Vector3 HitbackVelocity(Vector3 velocity, Vector3 hitbackNormal, float hitback)
  {
    var hitVel = hitbackNormal * hitback;
    var hitVelLength = hitVel.sqrMagnitude;
    var velLength = velocity.sqrMagnitude;
    var returnedVel = velLength >= hitVelLength ? velocity + hitVel : hitVel;
    return returnedVel;
  }

  public static Quaternion RotateByNormal(Vector3 normal, Vector3 axis, float additive = 360f)
  {
    var rot = additive - Mathf.Atan2(normal.z, normal.x) * Mathf.Rad2Deg;
    var vectRot = axis * rot;
    var qtrn = Quaternion.Euler(vectRot);
    return qtrn;
  }

  public static float DistanceFromTarget(Vector3 targetPosition, Vector3 position)
  {
    return (targetPosition - position).magnitude;
  }

  public static Blood BleedOutAtPoint(Blood bloodPrefab, Vector3 normal, Vector3 bleedPoint)
  {
    var bloodNrml = normal;
    var rot = 360f - Mathf.Atan2(bloodNrml.z, bloodNrml.x) * Mathf.Rad2Deg;
    var bloodIns = Object.Instantiate<Blood>(bloodPrefab, bleedPoint, Quaternion.Euler(0f, rot, 0f));
    Object.Destroy(bloodIns.gameObject, bloodIns.particleSystem.main.startLifetimeMultiplier);
    return bloodIns;
  }

  public static Blood BleedOut(Blood bloodPrefab, Quaternion rotation, Vector3 bleedPoint)
  {
    var headRot = rotation;
    var headEuler = headRot.eulerAngles;
    var bloodInsRot = Quaternion.Euler(headEuler.x, headEuler.y + 90f, headEuler.z);
    var bloodIns = Object.Instantiate<Blood>(bloodPrefab, bleedPoint, bloodInsRot);
    Object.Destroy(bloodIns.gameObject, bloodIns.particleSystem.main.startLifetimeMultiplier);
    return bloodIns;
  }

  public static Vector3 GetDirection(Transform transform, Vector3 axis)
  {
    return transform.rotation * axis;
  }

  public static Vector3 TranslateByMouseInsideScreen(float mouseX, float mouseY, Vector3 translatedPoint, Camera theCamera, float offsetZ = 1f, bool global = false)
  {
    var mousePoint = new Vector3(mouseX, 0f, mouseY);
    var newPoint = translatedPoint + mousePoint;
    if (global)
    {
      newPoint += theCamera.transform.position;
    }
    var min = theCamera.ScreenToWorldPoint(Vector3.zero);
    var max = theCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    var point = GetClampPoint(Point.FromVector3(newPoint), Point.FromVector3(min), Point.FromVector3(max));
    return Point.ToVector3(Vector3.up * offsetZ, point);
  }

  public static Vector3 TranslateByMouseInsideScreen(float mouseX, float mouseY, Vector3 translatedPoint, Camera theCamera, bool locally, Vector3 localDeltaPosition, float offsetZ = 1f)
  {
    var mousePoint = new Vector3(mouseX, 0f, mouseY);
    var newPoint = translatedPoint + mousePoint;
    if (locally)
    {
      newPoint -= localDeltaPosition;
    }
    var min = theCamera.ScreenToWorldPoint(Vector3.zero);
    var max = theCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    var point = GetClampPoint(Point.FromVector3(newPoint), Point.FromVector3(min), Point.FromVector3(max));
    return Point.ToVector3(Vector3.up * offsetZ, point);
  }

  public static Point GetClampPoint(Point value, Point min, Point max)
  {
    return new Point(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
  }

  public static Point ArrayToPoint(float[] position)
  {
    return new Point(position[0], position[1]);
  }

  public static Quaternion RotateToDirection(Vector3 normalize, Vector3 axis, float additive = 360f, bool isBaseY = false)
  {
    var angle = VectorToAngle(normalize, additive, isBaseY);
    var rotateAroundAxis = axis * angle;
    var qtrn = Quaternion.Euler(rotateAroundAxis);
    return qtrn;
  }

  public static float VectorToAngle(Vector3 normalize, float additive = 360f, bool isBaseY = false)
  {
    return additive - Mathf.Atan2(isBaseY ? normalize.y : normalize.z, normalize.x) * Mathf.Rad2Deg;
  }

  public static float[] QuaternionToAnglesArray(Quaternion rotation)
  {
    var angles = rotation.eulerAngles;
    return new[] { angles.x, angles.y, angles.z };
  }

  public static float[] Vector3ToPositionArray(Vector3 position)
  {
    return new[] { position.x, position.z };
  }

  public static Vector3 PositionArrayToVector3(Vector3 toVector3, float[] position)
  {
    return new Vector3(position[0], toVector3.y, position[1]);
  }

  public static Quaternion AnglesArrayToQuaternion(float[] rotation)
  {
    return Quaternion.Euler(rotation[0], rotation[1], rotation[2]);
  }

  public static T Deserialize<T>(object data)
  {
    return JsonUtility.FromJson<T>(data.ToString());
  }

  public static T Deserialize<T>(string data)
  {
    return JsonUtility.FromJson<T>(data);
  }

  public static string ShortId()
  {
    var base64Guid = System.Convert.ToBase64String(System.Guid.NewGuid().ToByteArray());
    return base64Guid;
  }
}
