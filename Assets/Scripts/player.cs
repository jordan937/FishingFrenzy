using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    public Animator playerAnim;
    public bool isFishing;
    public bool poleBack;
    public bool throwBobber;
    public Transform fishingPoint;
    public GameObject bobber;
    public GameObject fish; // Assign in inspector
    public GameObject fishGamePanel; // Assign in inspector

    public TextMeshProUGUI scoreText; // Assign this in the inspector
    private int Score = 0;
    public minigame barMinigame;
    public TextMeshProUGUI winText;

    public bool inGame = false;

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
    // Pick a random score bonus from the set {10, 30, 50}
    int[] scoreOptions = { 10, 30, 50 };
    int randomIndex = UnityEngine.Random.Range(0, scoreOptions.Length); // Random index from 0 to 2
    int scoreToAdd = scoreOptions[randomIndex];
    Debug.Log("Fish game won! Score: " + scoreToAdd);
    // Add the score to the player's score

    Score += scoreToAdd;

    if (scoreText != null)
    {
        scoreText.text = "Score: " + Score.ToString();
    }

    if(Score >= 300)
    {
        Debug.Log("You won the game!");
        SceneManager.LoadScene(0); // Load the main menu scene;
    }

    inGame = true;
    winnerAnim = true;
    fish.gameObject.SetActive(true);

    if (winText != null)
    {
        winText.gameObject.SetActive(true);
        Invoke(nameof(HideWinText), 2f);
    }

    Invoke(nameof(ResetFishingState), 3f);
}

public void fishGameLossed()
{
    Debug.Log("Fish game lost!");
    inGame = true;
    playerAnim.Play("playerStill");
    Invoke(nameof(ResetFishingState), 1f); // Optional: delay for loss reaction
}

private void ResetFishingState()
{
    Debug.Log("Resetting fishing state...");
    fish.SetActive(false);
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

void HideWinText()
{
    if (winText != null)
    {
        winText.gameObject.SetActive(false);
    }
}

}
