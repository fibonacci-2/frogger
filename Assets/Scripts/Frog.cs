using UnityEngine;

public class Frog : MonoBehaviour
{
    public float gridSize = 1f;

    void Update()
    {
        // Only the player-controlled frog should read input and advance time
        if (!gameObject.CompareTag("Player"))
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * gridSize);
            TickManager.Instance?.AdvanceTick();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * gridSize);
            TickManager.Instance?.AdvanceTick();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * gridSize);
            TickManager.Instance?.AdvanceTick();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * gridSize);
            TickManager.Instance?.AdvanceTick();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("car"))
            GameManager.Instance.Pause();
    }
}
