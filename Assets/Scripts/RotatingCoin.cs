using UnityEngine;

public class RotatingCoin : MonoBehaviour
{
    public Sprite[] coinSprites; // Array of coin sprites for animation
    public float frameRate = 10f; // Speed of the animation in frames per second

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;
    private bool isRotating = true; // Control flag for animation

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the coin object!");
        }

        if (coinSprites == null || coinSprites.Length == 0)
        {
            Debug.LogError("No coin sprites assigned to the RotatingCoin script!");
        }
    }

    private void Update()
    {
        if (!isRotating || coinSprites == null || coinSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % coinSprites.Length;
            spriteRenderer.sprite = coinSprites[currentFrame];
        }
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }
}
