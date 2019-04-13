using UnityEngine;

public class Beam : MonoBehaviour
{
    public float speed;
    public float distance;
    public float lifetime;
    public bool free;
    [System.NonSerialized]
	public Transform owner;
    
    public virtual void Use (Vector3 pos)
    {
           
    }
}
