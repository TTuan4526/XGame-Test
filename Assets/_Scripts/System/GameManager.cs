using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;
    private void Awake()
    {
        ins = this;
    }

    private Board gameBoard;
    private Spawner spawner;
    private Shape activeShape;
    private Ghost ghost;
    private Holder holder;

    private float dropInterval = 1f;
    private float dropIntervalModded;

    private float timeToDrop;
    private float timeToNextKeyLeftRight;
    private float timeToNextKeyDown;
    private float timeToNextKeyRotate;

    private float timeToNextSwipeLeftRight;
    private float timeToNextSwipeDown;

    [Range(0.02f, 1)]
    public float keyRepeatRateLeftRight = 0.1f;

    [Range(0.02f, 1)]
    public float keyRepeatRateDown = 0.05f;

    [Range(0.02f, 1)]
    public float keyRepeatRateRotate = 0.05f;

    [Range(0.02f, 1)]
    public float swipeRepeatRateLeftRight = 0.25f;

    [Range(0.02f, 1)]
    public float swipeRepeatRateDown = 0.05f;


    public IconToggle rotateIconToggle;
    public ParticlePlayer gameOverFx;

    
    private bool rotateClockwise = true;

    public bool gameOver = false;
    public bool isPaused = false;

    private enum Direction { none, left, right, up, down }

    private Direction swipeDirection = Direction.none;
    private Direction swipeEndDirection = Direction.none;

    private void OnEnable()
    {
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.SwipeEndEvent += SwipeEndHandler;
    }

    private void OnDisable()
    {
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.SwipeEndEvent -= SwipeEndHandler;
    }

    public void Start()
    {
        activeShape = null;

        gameBoard = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner>();

        ghost = GameObject.FindObjectOfType<Ghost>();
        holder = GameObject.FindObjectOfType<Holder>();

        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        timeToNextKeyRotate = Time.time + keyRepeatRateRotate;
        timeToNextKeyDown = Time.time + keyRepeatRateDown;

        if (!spawner)
        {
            Debug.Log("Error");
        }
        else
        {
            if (activeShape == null)
            {
                activeShape = spawner.SpawnShape();
            }

            spawner.transform.position = Vectorf.Round(spawner.transform.position);
        }

        dropIntervalModded = dropInterval;
    }

    private void MoveRight()
    {
        activeShape.MoveRight();
        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        timeToNextSwipeLeftRight = Time.time + swipeRepeatRateLeftRight;

        if (!gameBoard.IsValidPosition(activeShape))
        {
            activeShape.MoveLeft();
            PlaySound(SoundManager.ins.errorSound, 0.5f);
        }
        else
        {
            PlaySound(SoundManager.ins.moveSound, 0.5f);
        }
    }

    private void MoveLeft()
    {
        activeShape.MoveLeft();
        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        timeToNextSwipeLeftRight = Time.time + swipeRepeatRateLeftRight;

        if (!gameBoard.IsValidPosition(activeShape))
        {
            activeShape.MoveRight();
            PlaySound(SoundManager.ins.errorSound, 0.5f);
        }
        else
        {
            PlaySound(SoundManager.ins.moveSound, 0.5f);
        }
    }

    private void Rotate()
    {
        activeShape.RotateClockwise(rotateClockwise);
        timeToNextKeyRotate = Time.time + keyRepeatRateRotate;

        if (!gameBoard.IsValidPosition(activeShape))
        {
            activeShape.RotateClockwise(!rotateClockwise);
            PlaySound(SoundManager.ins.errorSound, 0.5f);
        }
        else
        {
            PlaySound(SoundManager.ins.moveSound, 0.5f);
        }
    }

    private void MoveDown()
    {
        timeToDrop = Time.time + dropIntervalModded;
        timeToNextKeyDown = Time.time + keyRepeatRateDown;
        timeToNextSwipeDown = Time.time + swipeRepeatRateDown;

        if (activeShape)
        {
            activeShape.MoveDown();

            if (!gameBoard.IsValidPosition(activeShape))
            {
                if (gameBoard.IsOverLimit(activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }
        }
    }

    private void PlayerInput()
    {
        if (!gameBoard || !spawner)
        {
            return;
        }

        if ((Input.GetButton("MoveRight") && Time.time > timeToNextKeyLeftRight) ||
             Input.GetButtonDown("MoveRight"))
        {
            MoveRight();
        }
        else if ((Input.GetButton("MoveLeft") && Time.time > timeToNextKeyLeftRight) ||
                  Input.GetButtonDown("MoveLeft"))
        {
            MoveLeft();
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > timeToNextKeyRotate)
        {
            Rotate();
        }
        else if ((Input.GetButton("MoveDown") && Time.time > timeToNextKeyDown) ||
                  Time.time > timeToDrop)
        {
            MoveDown();
        }
        else if ((swipeDirection == Direction.right && Time.time > timeToNextSwipeLeftRight) ||
                  swipeEndDirection == Direction.right)
        {
            MoveRight();

            swipeDirection = Direction.none;
            swipeEndDirection = Direction.none;
        }
        else if ((swipeDirection == Direction.left && Time.time > timeToNextSwipeLeftRight) ||
                  swipeEndDirection == Direction.left)
        {
            MoveLeft();

            swipeDirection = Direction.none;
            swipeEndDirection = Direction.none;
        }
        else if (swipeEndDirection == Direction.up)
        {
            Rotate();

            swipeEndDirection = Direction.none;
        }
        else if (swipeDirection == Direction.down && Time.time > timeToNextSwipeDown)
        {
            MoveDown();

            swipeDirection = Direction.none;
        }
        else if (Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotationDirection();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }
    }

    private void GameOver()
    {
        activeShape.MoveUp();

        UIManager.ins.GameOverScreen.SetActive(true);

        DataManager.ins.SaveScore();

        StartCoroutine(GameOverRoutine());

        PlaySound(SoundManager.ins.gameOverSound, 5f);
        PlaySound(SoundManager.ins.gameOverVocalClip, 5f);

        gameOver = true;
    }

    private IEnumerator GameOverRoutine()
    {
        if (gameOverFx)
        {
            gameOverFx.Play();
        }

        yield return new WaitForSeconds(0.1f);
    }

    private void LandShape()
    {
        timeToNextKeyLeftRight = Time.time;
        timeToNextKeyRotate = Time.time;
        timeToNextKeyDown = Time.time;

        activeShape.MoveUp();
        gameBoard.StoreShapeInGrid(activeShape);

        if (ghost)
        {
            ghost.Reset();
        }

        if (holder)
        {
            holder.canRelease = true;
        }

        activeShape = spawner.SpawnShape();

        gameBoard.StartCoroutine(gameBoard.ClearAllRows());

        PlaySound(SoundManager.ins.dropSound, 0.75f);

        if (gameBoard.completedRows > 0)
        {
            DataManager.ins.ScoreLines(gameBoard.completedRows);

            PlaySound(SoundManager.ins.clearRowSound);
        }
    }

    private void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip && SoundManager.ins.fxEnabled)
        {
            AudioSource.PlayClipAtPoint(
                clip, Camera.main.transform.position,
                Mathf.Clamp(SoundManager.ins.fxVolume * volumeMultiplier, 0.05f, 1f));
        }
    }

    private void Update()
    {
        if (!gameBoard || !spawner || !activeShape || gameOver)
        {
            return;
        }

        PlayerInput();
    }

    private void LateUpdate()
    {
        if (ghost)
        {
            ghost.DrawGhost(activeShape, gameBoard);
        }
    }

    private void SwipeHandler(Vector2 swipeMovement)
    {
        swipeDirection = GetDirection(swipeMovement);
    }

    private void SwipeEndHandler(Vector2 swipeMovement)
    {
        swipeEndDirection = GetDirection(swipeMovement);
    }

    private Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDirection = Direction.none;

        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDirection = swipeMovement.x >= 0 ? Direction.right : Direction.left;
        }
        else
        {
            swipeDirection = swipeMovement.y >= 0 ? Direction.up : Direction.down;
        }

        return swipeDirection;
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Endless");
    }

    public void ToggleRotationDirection()
    {
        rotateClockwise = !rotateClockwise;

        if (rotateIconToggle)
        {
            rotateIconToggle.ToggleIcon(rotateClockwise);
        }
    }

    public void Hold()
    {
        if (!holder)
        {
            return;
        }

        if (!holder.heldShape)
        {
            holder.Catch(activeShape);
            activeShape = spawner.SpawnShape();
            PlaySound(SoundManager.ins.holdSound);
        }
        else if (holder.canRelease)
        {
            Shape shape = activeShape;
            activeShape = holder.Release();
            activeShape.transform.position = spawner.transform.position;
            holder.Catch(shape);
            PlaySound(SoundManager.ins.holdSound);
        }
        else
        {
            PlaySound(SoundManager.ins.errorSound);
        }

        if (ghost)
        {
            ghost.Reset();
        }
    }
}
