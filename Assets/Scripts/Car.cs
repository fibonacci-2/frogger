using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 5f;
    public int direction = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }
}
