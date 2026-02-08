using UnityEngine;

public class Frog : MonoBehaviour
{
    public float gridSize = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            transform.Translate(Vector3.up * gridSize);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            transform.Translate(Vector3.down * gridSize);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            transform.Translate(Vector3.left * gridSize);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            transform.Translate(Vector3.right * gridSize);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("car"))
            GameManager.Instance.Pause();
    }
}
