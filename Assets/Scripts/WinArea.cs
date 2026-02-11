using UnityEngine;

public class WinArea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<Frog>() != null)
            Debug.Log("You Win!");
    }
}
