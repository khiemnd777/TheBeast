using UnityEngine;

public class Utility
{
  public static Vector3 CameraInBound(Camera camera, BoxCollider bound, Vector3 position)
  {
    var min = bound.bounds.min;
    var max = bound.bounds.max;
    var halfHeight = camera.orthographicSize;
    var halfWidth = halfHeight * Screen.width / Screen.height;
    var x = Mathf.Clamp(position.x, min.x + halfWidth, max.x - halfWidth);
    var z = Mathf.Clamp(position.z, min.z + halfHeight, max.z - halfHeight);
    return new Vector3(x, position.y, z);
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

  public static Vector3 TranslateByMouseInsideScreen(float mouseX, float mouseY, Vector3 translatedPoint, Camera theCamera, float offsetZ = 1f)
  {
    var mousePoint = new Vector3(mouseX, 0f, mouseY);
    var newPoint = translatedPoint + mousePoint;
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
}
