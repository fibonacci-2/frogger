using UnityEngine;

// WinArea: triggers the win condition when the player or a frog
// enters the area's trigger collider.
public class WinArea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<Frog>() != null)
            Debug.Log("You Win!");
    }
}
