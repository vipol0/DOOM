using System.Collections.Generic;
using UnityEngine;

public class WaveManagerConsole : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;

    [Header("Console")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;
    [SerializeField] private bool openOnStart;

    [Header("Pause")]
    [SerializeField] private bool pauseGame = true;

    private readonly List<string> output = new();

    private string command = string.Empty;

    private bool isOpen;
    private float previousTimeScale = 1f;

    private Vector2 scrollPosition;

    private void Awake()
    {
        isOpen = false;

        Print("Wave Manager Console");
        Print("Type 'help' for commands.");

        if (openOnStart)
            Open();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }
    }

    private void OnDisable()
    {
        if (isOpen)
            Close();
    }

    private void OnGUI()
    {
        if (!isOpen)
            return;

        DrawConsole();
    }

    private void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    private void Open()
    {
        isOpen = true;

        ShowCursor();

        if (pauseGame)
            PauseGame();

        FocusInput();
    }

    private void Close()
    {
        isOpen = false;

        RestoreCursor();

        if (pauseGame)
            ResumeGame();

        GUI.FocusControl(null);
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RestoreCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = previousTimeScale;
    }

    private void DrawConsole()
    {
        const float width = 700f;
        const float height = 400f;

        var rect = new Rect(
            20f,
            20f,
            width,
            height
        );

        GUI.Box(rect, GUIContent.none);

        GUILayout.BeginArea(
            new Rect(
                rect.x + 10f,
                rect.y + 10f,
                rect.width - 20f,
                rect.height - 20f
            )
        );

        GUILayout.Label(
            "WAVE MANAGER CONSOLE",
            GUI.skin.box
        );

        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition,
            GUI.skin.box,
            GUILayout.Height(320f)
        );

        foreach (var line in output)
        {
            GUILayout.Label(line);
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        GUI.SetNextControlName("CommandInput");

        command = GUILayout.TextField(
            command,
            GUILayout.Height(30f)
        );

        if (GUILayout.Button(
                "Execute",
                GUILayout.Width(100f),
                GUILayout.Height(30f)))
        {
            ExecuteCommand();
        }

        GUILayout.EndHorizontal();

        GUILayout.Label(
            $"Current wave: {GetCurrentWaveText()}",
            GUI.skin.box
        );

        GUILayout.EndArea();
    }

    private void ExecuteCommand()
    {
        var input = command.Trim();

        if (string.IsNullOrEmpty(input))
            return;

        Print($"> {input}");

        command = string.Empty;

        var parts = input.Split(
            ' ',
            System.StringSplitOptions.RemoveEmptyEntries
        );

        var commandName = parts[0].ToLowerInvariant();

        switch (commandName)
        {
            case "help":
                ShowHelp();
                break;

            case "wave":
                ExecuteWave(parts);
                break;

            case "next":
                StartNextWave();
                break;

            case "previous":
            case "prev":
                StartPreviousWave();
                break;

            case "restart":
                RestartWave();
                break;

            case "break":
                StartBreak();
                break;

            case "skipbreak":
                SkipBreak();
                break;

            case "clear":
                output.Clear();
                break;

            case "status":
                ShowStatus();
                break;

            default:
                Print(
                    $"Unknown command: {commandName}"
                );

                Print(
                    "Type 'help' for available commands."
                );

                break;
        }

        FocusInput();
    }

    private void ExecuteWave(string[] parts)
    {
        if (parts.Length < 2)
        {
            Print("Usage: wave <number>");
            return;
        }

        if (!int.TryParse(parts[1], out var waveNumber))
        {
            Print("Wave number must be an integer.");
            return;
        }

        var waveIndex = waveNumber - 1;

        if (waveIndex < 0 ||
            waveIndex >= waveManager.WaveCount)
        {
            Print(
                $"Wave {waveNumber} does not exist."
            );

            Print(
                $"Available waves: 1-{waveManager.WaveCount}"
            );

            return;
        }

        waveManager.StartWave(waveIndex);

        Print(
            $"Started wave {waveNumber}."
        );
    }

    private void StartNextWave()
    {
        var nextIndex =
            waveManager.CurrentWaveIndex + 1;

        if (nextIndex >= waveManager.WaveCount)
        {
            Print("There is no next wave.");
            return;
        }

        waveManager.StartWave(nextIndex);

        Print(
            $"Started wave {nextIndex + 1}."
        );
    }

    private void StartPreviousWave()
    {
        var previousIndex =
            waveManager.CurrentWaveIndex - 1;

        if (previousIndex < 0)
        {
            Print("There is no previous wave.");
            return;
        }

        waveManager.StartWave(previousIndex);

        Print(
            $"Started wave {previousIndex + 1}."
        );
    }

    private void RestartWave()
    {
        if (waveManager.CurrentWaveIndex < 0)
        {
            Print("No active wave.");
            return;
        }

        var waveNumber =
            waveManager.CurrentWaveIndex + 1;

        waveManager.RestartCurrentWave();

        Print(
            $"Restarted wave {waveNumber}."
        );
    }

    private void StartBreak()
    {
        waveManager.StartBreak();

        Print("Break started.");
    }

    private void SkipBreak()
    {
        waveManager.SkipBreak();

        Print("Break skipped.");
    }

    private void ShowStatus()
    {
        Print(
            $"Wave: {GetCurrentWaveText()}"
        );

        Print(
            $"Wave active: {waveManager.IsWaveActive}"
        );

        Print(
            $"Break active: {waveManager.IsBreakActive}"
        );

        Print(
            $"Total waves: {waveManager.WaveCount}"
        );

        Print(
            $"Time scale: {Time.timeScale}"
        );
    }

    private void ShowHelp()
    {
        Print("Available commands:");
        Print("wave <number>  - start specific wave");
        Print("next           - start next wave");
        Print("previous       - start previous wave");
        Print("restart        - restart current wave");
        Print("break          - start break");
        Print("skipbreak      - skip current break");
        Print("status         - show WaveManager status");
        Print("clear          - clear console");
        Print("help           - show this message");
    }

    private string GetCurrentWaveText()
    {
        if (waveManager.CurrentWaveIndex < 0)
            return "None";

        return (
            waveManager.CurrentWaveIndex + 1
        ).ToString();
    }

    private void Print(string message)
    {
        output.Add(message);

        if (output.Count > 100)
            output.RemoveAt(0);

        scrollPosition.y = float.MaxValue;
    }

    private void FocusInput()
    {
        if (!isOpen)
            return;

        GUI.FocusControl("CommandInput");
    }
}