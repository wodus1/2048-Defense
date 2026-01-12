using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

enum Direction { left, right, up, down }
enum InputState { Idle, Dragging }

public class GameManager : MonoBehaviour //게임 매니저(2048 로직)
{
    private struct tileMove
    {
        public int value;
        public int startRow, startCol;
        public int endRow, endCol;
        public bool merged;
    }

    [SerializeField] private SubSystemsManager subSystemsManager;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private List<RectTransform> tilePos = new List<RectTransform>();
    [SerializeField] private List<TileUI> tiles = new List<TileUI>();
    
    private int[,] map = new int[4, 4];
    private TileUI[,] tileMap = new TileUI[4, 4];
    private List<tileMove> tileMoves = new List<tileMove>();
    private List<(int matrixIdx, int value)> moveLines = new List<(int matrixIdx, int value)>(4);
    private int[] newLines = new int[4];
    List<(int r, int c)> emptyTiles = new List<(int r, int c)>();
    private bool[,] hasMainTile = new bool[4, 4];

    private InputState inputState = InputState.Idle;
    private Vector2 startTouchPos;
    private float dragThreshold = 50f;
    private LevelSystem levelSystem;

    public bool InputLocked = false;
    public bool IsPause;
    public SubSystemsManager SubSystemsManager => subSystemsManager;

    void Awake()
    {
        InitGameSetting();
    }

    void Update()
    {
        if (InputLocked)
            return;

        if (IsPause)
            return;

        switch (inputState)
        {
            case InputState.Idle:
                DragStart();
                break;

            case InputState.Dragging:
                DragEnd();
                break;
        }
    }

    public void InitGameSetting()
    {
        Resume();
        InitTiles();
        subSystemsManager.Initialize(this);
        levelSystem = subSystemsManager.GetSubSystem<LevelSystem>();
        gameOverUI.Initialize(this);
    }

    public void OnGameOver()
    {
        subSystemsManager.Deinitialize();
        gameOverUI.GameOverPanel.SetActive(true);
        Pause();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        IsPause = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        IsPause = true;
    }

    public void ClearTilesExceptMax()
    {
        int row = 0;
        int col = 0;
        int max = 0;

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (map[r, c] > max)
                {
                    max = map[r, c];
                    row = r;
                    col = c;
                }
                map[r, c] = 0;
            }
        }

        map[row, col] = max;

        RefreshTiles();
    }

    private void InitTiles()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                map[r, c] = 0;
            }
        }

        RefreshTiles();

        SpawnRandomTile();
        SpawnRandomTile();
    }

    private void RefreshTiles()
    {
        foreach (var tile in tiles)
        {
            tile.Hide();
            tile.SetValue(0);
        }

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                tileMap[r, c] = null;
            }
        }

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (map[r, c] != 0)
                {
                    SpawnTile(r, c, map[r, c]);
                }
            }
        }
    }

    private void DragStart()
    {
        if (Pointer.current == null)
            return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            startTouchPos = Pointer.current.position.ReadValue();

            inputState = InputState.Dragging;
        }
    }

    private void DragEnd()
    {
        if (Pointer.current == null)
            return;

        if (!Pointer.current.press.isPressed)
        {
            inputState = InputState.Idle;
            return;
        }

        Vector2 currentPos = Pointer.current.position.ReadValue();
        Vector2 delta = currentPos - startTouchPos;

        if (delta.magnitude < dragThreshold)
            return;

        Direction dir;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            dir = (delta.x > 0) ? Direction.right : Direction.left;
        }
        else
        {
            dir = (delta.y > 0) ? Direction.up : Direction.down;
        }

        bool moved = MoveTile(dir);

        if (moved)
        {
            InputLocked = true;
            PlayTileAnimation();
        }

        inputState = InputState.Idle;
    }

    private bool MoveTile(Direction dir)
    {
        bool moved = false;
        tileMoves.Clear();

        switch (dir)
        {
            case Direction.left:
                for (int r = 0; r < 4; r++)
                {
                    if (MoveLineLeft(r))
                        moved = true;
                }

                break;
            case Direction.right:
                for (int r = 0; r < 4; r++)
                {
                    if (MoveLineRight(r))
                        moved = true;
                }

                break;
            case Direction.up:
                for (int c = 0; c < 4; c++)
                {
                    if (MoveLineUp(c))
                        moved = true;
                }

                break;
            case Direction.down:
                for (int c = 0; c < 4; c++)
                {
                    if (MoveLineDown(c))
                        moved = true;
                }

                break;
        }

        return moved;
    }

    private void SpawnRandomTile()
    {
        emptyTiles.Clear();
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (map[r, c] == 0)
                    emptyTiles.Add((r, c));
            }
        }

        if (emptyTiles.Count == 0)
        {
            return;
        }

        var pos = emptyTiles[UnityEngine.Random.Range(0, emptyTiles.Count)];

        float v = UnityEngine.Random.Range(0f, 1f);
        int value = (v < 0.5f) ? 2 : 4;
        map[pos.r, pos.c] = value;

        SpawnTile(pos.r, pos.c, value);
    }


    #region 타일 이동
    private bool MoveLineRight(int r)
    {
        //0이 아닌 타일 저장
        moveLines.Clear();
        for (int c = 3; c >= 0; c--)
        {
            int v = map[r, c];
            if (v != 0)
                moveLines.Add((c, v));
        }

        if (moveLines.Count == 0)
            return false;

        Array.Clear(newLines, 0, newLines.Length);
        int colIdx = 3;
        int idx = 0;

        while (idx < moveLines.Count)
        {
            int c1 = moveLines[idx].matrixIdx;
            int value = moveLines[idx].value;

            if (idx + 1 < moveLines.Count && moveLines[idx + 1].value == value)
            {
                int c2 = moveLines[idx + 1].matrixIdx;
                int mergedValue = value * 2;

                newLines[colIdx] = mergedValue;
                levelSystem.AddExp(mergedValue);

                tileMoves.Add(new tileMove
                {
                    startRow = r,
                    startCol = c1,
                    endRow = r,
                    endCol = colIdx,
                    value = mergedValue,
                    merged = true
                });

                tileMoves.Add(new tileMove
                {
                    startRow = r,
                    startCol = c2,
                    endRow = r,
                    endCol = colIdx,
                    value = mergedValue,
                    merged = true
                });

                idx += 2;
                colIdx--;
            }
            else
            {
                newLines[colIdx] = value;

                tileMoves.Add(new tileMove
                {
                    startRow = r,
                    startCol = c1,
                    endRow = r,
                    endCol = colIdx,
                    value = value,
                    merged = false
                });

                idx++;
                colIdx--;
            }
        }

        bool changed = false;
        for (int c = 0; c < 4; c++)
        {
            if (map[r, c] != newLines[c])
            {
                map[r, c] = newLines[c];
                changed = true;
            }
        }

        return changed;
    }

    private bool MoveLineLeft(int r)
    {
        //0이 아닌 타일 저장
        moveLines.Clear();
        for (int c = 0; c < 4; c++)
        {
            int v = map[r, c];
            if (v != 0)
                moveLines.Add((c, v));
        }

        if (moveLines.Count == 0)
            return false;

        Array.Clear(newLines, 0, newLines.Length);
        int colIdx = 0;
        int idx = 0;

        while (idx < moveLines.Count)
        {
            int c1 = moveLines[idx].matrixIdx;
            int value = moveLines[idx].value;

            if (idx + 1 < moveLines.Count && moveLines[idx + 1].value == value)
            {
                int c2 = moveLines[idx + 1].matrixIdx;
                int mergedValue = value * 2;

                newLines[colIdx] = mergedValue;
                levelSystem.AddExp(mergedValue);

                tileMoves.Add(new tileMove
                {
                    startRow = r,
                    startCol = c1,
                    endRow = r,
                    endCol = colIdx,
                    value = mergedValue,
                    merged = true
                });

                tileMoves.Add(new tileMove
                {
                    startRow = r,
                    startCol = c2,
                    endRow = r,
                    endCol = colIdx,
                    value = mergedValue,
                    merged = true
                });

                idx += 2;
                colIdx++;
            }
            else
            {
                newLines[colIdx] = value;

                tileMoves.Add(new tileMove
                {
                    startRow = r,
                    startCol = c1,
                    endRow = r,
                    endCol = colIdx,
                    value = value,
                    merged = false
                });

                idx++;
                colIdx++;
            }
        }

        bool changed = false;
        for (int c = 0; c < 4; c++)
        {
            if (map[r, c] != newLines[c])
            {
                map[r, c] = newLines[c];
                changed = true;
            }
        }

        return changed;
    }

    private bool MoveLineUp(int c)
    {
        //0이 아닌 타일 저장
        moveLines.Clear();
        for (int r = 0; r < 4; r++)
        {
            int v = map[r, c];
            if (v != 0)
                moveLines.Add((r, v));
        }

        if (moveLines.Count == 0)
            return false;

        Array.Clear(newLines, 0, newLines.Length);
        int rowIdx = 0;
        int idx = 0;

        while (idx < moveLines.Count)
        {
            int r1 = moveLines[idx].matrixIdx;
            int value = moveLines[idx].value;

            int targetRow = rowIdx;

            if (idx + 1 < moveLines.Count && moveLines[idx + 1].value == value)
            {
                int r2 = moveLines[idx + 1].matrixIdx;
                int mergedValue = value * 2;

                newLines[targetRow] = mergedValue;
                levelSystem.AddExp(mergedValue);

                tileMoves.Add(new tileMove
                {
                    startRow = r1,
                    startCol = c,
                    endRow = targetRow,
                    endCol = c,
                    value = mergedValue,
                    merged = true
                });

                tileMoves.Add(new tileMove
                {
                    startRow = r2,
                    startCol = c,
                    endRow = targetRow,
                    endCol = c,
                    value = mergedValue,
                    merged = true
                });

                idx += 2;
                rowIdx++;
            }
            else
            {
                newLines[targetRow] = value;

                tileMoves.Add(new tileMove
                {
                    startRow = r1,
                    startCol = c,
                    endRow = targetRow,
                    endCol = c,
                    value = value,
                    merged = false
                });

                idx++;
                rowIdx++;
            }
        }

        bool changed = false;
        for (int r = 0; r < 4; r++)
        {
            if (map[r, c] != newLines[r])
            {
                map[r, c] = newLines[r];
                changed = true;
            }
        }

        return changed;
    }

    private bool MoveLineDown(int c)
    {
        //0이 아닌 타일 저장
        moveLines.Clear();
        for (int r = 3; r >= 0; r--)
        {
            int v = map[r, c];
            if (v != 0)
                moveLines.Add((r, v));
        }

        if (moveLines.Count == 0)
            return false;

        Array.Clear(newLines, 0, newLines.Length);
        int rowIdx = 3;
        int idx = 0;

        while (idx < moveLines.Count)
        {
            int r1 = moveLines[idx].matrixIdx;
            int value = moveLines[idx].value;

            if (idx + 1 < moveLines.Count && moveLines[idx + 1].value == value)
            {
                int r2 = moveLines[idx + 1].matrixIdx;
                int mergedValue = value * 2;

                newLines[rowIdx] = mergedValue;
                levelSystem.AddExp(mergedValue);

                tileMoves.Add(new tileMove
                {
                    startRow = r1,
                    startCol = c,
                    endRow = rowIdx,
                    endCol = c,
                    value = mergedValue,
                    merged = true
                });

                tileMoves.Add(new tileMove
                {
                    startRow = r2,
                    startCol = c,
                    endRow = rowIdx,
                    endCol = c,
                    value = mergedValue,
                    merged = true
                });

                idx += 2;
                rowIdx--;
            }
            else
            {
                newLines[rowIdx] = value;

                tileMoves.Add(new tileMove
                {
                    startRow = r1,
                    startCol = c,
                    endRow = rowIdx,
                    endCol = c,
                    value = value,
                    merged = false
                });

                idx++;
                rowIdx--;
            }
        }

        bool changed = false;
        for (int r = 0; r < 4; r++)
        {
            if (map[r, c] != newLines[r])
            {
                map[r, c] = newLines[r];
                changed = true;
            }
        }

        return changed;
    }
    #endregion

    private TileUI GetTile()
    {
        foreach (var tile in tiles)
        {
            if (!tile.IsUse)
                return tile;
        }

        return null;
    }

    private void SpawnTile(int r, int c, int value)
    {
        var tile = GetTile();
        if (tile == null)
            return;

        int index = r * 4 + c;
        var targetPos = tilePos[index];

        tile.RectTransform.anchoredPosition = targetPos.anchoredPosition;
        tile.Show();
        tile.SetValue(value, false);
        tile.PlaySpawn();

        tileMap[r, c] = tile;
    }

    private void PlayTileAnimation()
    {
        Array.Clear(hasMainTile, 0, hasMainTile.Length);

        Sequence seq = DOTween.Sequence();

        foreach (var move in tileMoves)
        {
            var tile = tileMap[move.startRow, move.startCol];
            if (tile == null)
                continue;

            int startRow = move.startRow, startCol = move.startCol;
            int endRow = move.endRow, endCol = move.endCol;
            int value = move.value;

            int startIndex = startRow * 4 + startCol;
            int endIndex = endRow * 4 + endCol;

            var startPos = tilePos[startIndex].anchoredPosition;
            var endPos = tilePos[endIndex].anchoredPosition;

            tile.RectTransform.anchoredPosition = startPos;

            bool isSecondMerged = move.merged && hasMainTile[endRow, endCol];

            tileMap[startRow, startCol] = null;

            if (!isSecondMerged)
            {
                hasMainTile[endRow, endCol] = true;
                tileMap[endRow, endCol] = tile;
            }

            Tween tween = tile.PlayMove(
                endPos,
                0.15f,
                isSecondMerged,
                value
            );

            seq.Join(tween);
        }

        seq.OnComplete(TileAnimComplete);
    }

    private void TileAnimComplete()
    {
        SpawnRandomTile();
        InputLocked = false;
    }
}