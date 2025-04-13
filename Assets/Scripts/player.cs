using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator playerAnim;
    public bool isFishing;
    public bool poleBack;
    public bool throwBobber;
    public Transform fishingPoint;
    public GameObject bobber;
    public GameObject fishGamePanel; // Assign in inspector

    public minigame barMinigame;

    public bool inGame = false;
    public float moveSpeed = 5f; // Movement speed for 2D

    private bool minigameStarted = false;
    private bool bobberInWater = false;
    private bool winnerAnim = false;

    public float targetTime = 0.0f;
    public float savedTargetTime;
    public float extraBobberDistance;

    public float timeTillCatch = 0.0f;

    void Start()
    {
        ResetFishingState();

        if (bobber != null)
        {
            bobber.SetActive(false); // Ensure bobber starts inactive
        }
    }

    void Update()
    {
        // --------------------------
        // Optional 2D WASD Movement
        // --------------------------
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;

        if (!isFishing)
        {
            transform.Translate(move);

            if (move != Vector3.zero)
            {
                playerAnim.Play("playerWalk");

                // Flip sprite
                if (move.x > 0)
                    transform.localScale = new Vector3(8, 8, 8);
                else if (move.x < 0)
                    transform.localScale = new Vector3(-8, 8, 8);
            }
            else
            {
                playerAnim.Play("playerStill");
            }
        }

        // --------------------------
        // Start casting fishing pole
        // --------------------------
        if (Input.GetKeyDown(KeyCode.Space) && !isFishing && !winnerAnim)
        {
            poleBack = true;
        }

        // Pull back animation logic
        if (poleBack)
        {
            playerAnim.Play("playerSwingBack");
            targetTime += Time.deltaTime;
        }

        // --------------------------
        // Throw bobber
        // --------------------------
        if (Input.GetKeyUp(KeyCode.Space) && poleBack && !isFishing)
        {
            Debug.Log("Throw bobber");
            isFishing = true;
            throwBobber = true;
            savedTargetTime = targetTime;
            extraBobberDistance = targetTime >= 3 ? 3 : targetTime;

            poleBack = false;
            targetTime = 0.0f;
        }

        // Position adjustment before throw
        if (throwBobber && isFishing)
        {
            Vector3 temp = new Vector3(extraBobberDistance, 0, 0);
            if (fishingPoint != null)
            {
                fishingPoint.transform.position += temp;

                if (bobber != null)
                {
                    bobber.transform.position = fishingPoint.position;
                    bobber.transform.rotation = fishingPoint.rotation;
                    bobber.SetActive(true);
                    bobberInWater = true;
                }

                fishingPoint.transform.position -= temp;
            }

            throwBobber = false;
        }

        // --------------------------
        // While fishing
        // --------------------------
        if (isFishing)
        {
            playerAnim.Play("playerFishing");
            timeTillCatch += Time.deltaTime;

            if (timeTillCatch >= 3f && !minigameStarted)
            {
                StartFishMinigame();
            }

            // Cancel fishing
            if (Input.GetKeyDown(KeyCode.P) && !inGame && !minigameStarted)
            {
                playerAnim.Play("playerStill");
                ResetFishingState();
            }
        }
    }

public void fishGameWon()
{
    Debug.Log("Fish game won!");
    inGame = true;
    playerAnim.Play("playerWonFish");
    Invoke(nameof(ResetFishingState), 1.5f); // Delay reset to allow win animation
}

public void fishGameLossed()
{
    Debug.Log("Fish game lost!");
    inGame = true;
    playerAnim.Play("playerStill");
    Invoke(nameof(ResetFishingState), 1.0f); // Optional: delay for loss reaction
}

private void ResetFishingState()
{
    poleBack = false;
    throwBobber = false;
    isFishing = false;
    targetTime = 0.0f;
    savedTargetTime = 0.0f;
    extraBobberDistance = 0.0f;
    timeTillCatch = 0.0f;
    minigameStarted = false;
    inGame = false;
    winnerAnim = false;

    if (bobber != null)
    {
        bobber.SetActive(false);
        bobberInWater = false;
    }

    if (fishGamePanel != null)
    {
        fishGamePanel.SetActive(false);
    }

    Debug.Log("Ready to fish again.");
}

    private void StartFishMinigame()
{
    minigameStarted = true;
    if (fishGamePanel != null)
    {
        fishGamePanel.SetActive(true);
    }

    if (barMinigame != null)
    {
        barMinigame.OnResult = (bool success) =>
        {
            if (success)
                fishGameWon();
            else
                fishGameLossed();
        };
    }

    Debug.Log("Bar minigame started!");
}

    private void ResolveFishCatch()
    {
        bool caughtFish = UnityEngine.Random.value > 0.5f;

        if (caughtFish)
        {
            fishGameWon();
        }
        else
        {
            fishGameLossed();
        }
    }
}
