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
            tile.gameObject.SetActive(false);
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
            PlayTitleAnimation();
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
                Debug.Log(moved);
                break;
            case Direction.right:
                for (int r = 0; r < 4; r++)
                {
                    if (MoveLineRight(r))
                        moved = true;
                }
                Debug.Log(moved);
                break;
            case Direction.up:
                for (int c = 0; c < 4; c++)
                {
                    if (MoveLineUp(c))
                        moved = true;
                }
                Debug.Log(moved);
                break;
            case Direction.down:
                for (int c = 0; c < 4; c++)
                {
                    if (MoveLineDown(c))
                        moved = true;
                }
                Debug.Log(moved);
                break;
        }

        if(!moved)
        {
            for(int r = 0; r < 4; r++)
            {
                string line = "";
                for (int c = 0; c < 4; c++)
                {
                    line += map[r, c] + " ";
                }
                Debug.Log(line);
            }
        }

        return moved;
    }

    private void SpawnRandomTile()
    {
        List<(int r, int c)> empty = new List<(int r, int c)>();

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (map[r, c] == 0)
                    empty.Add((r, c));
            }
        }

        if (empty.Count == 0)
        {
            return;
        }

        var pos = empty[UnityEngine.Random.Range(0, empty.Count)];

        float v = UnityEngine.Random.Range(0f, 1f);
        int value = (v < 0.5f) ? 2 : 4;
        map[pos.r, pos.c] = value;

        SpawnTile(pos.r, pos.c, value);
    }


    #region 타일 이동
    private bool MoveLineRight(int r)
    {
        //0이 아닌 타일 저장
        List<(int col, int value)> lines = new List<(int col, int value)>();
        for (int c = 3; c >= 0; c--)
        {
            int v = map[r, c];
            if (v != 0)
                lines.Add((c, v));
        }

        if (lines.Count == 0)
            return false;

        int[] newLine = new int[4];
        int colIdx = 3;
        int idx = 0;

        while (idx < lines.Count)
        {
            int c1 = lines[idx].col;
            int value = lines[idx].value;

            if (idx + 1 < lines.Count && lines[idx + 1].value == value)
            {
                int c2 = lines[idx + 1].col;
                int mergedValue = value * 2;

                newLine[colIdx] = mergedValue;
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
                newLine[colIdx] = value;

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
            if (map[r, c] != newLine[c])
            {
                map[r, c] = newLine[c];
                changed = true;
            }
        }

        return changed;
    }

    private bool MoveLineLeft(int r)
    {
        //0이 아닌 타일 저장
        List<(int col, int value)> lines = new List<(int col, int value)>();
        for (int c = 0; c < 4; c++)
        {
            int v = map[r, c];
            if (v != 0)
                lines.Add((c, v));
        }

        if (lines.Count == 0)
            return false;

        int[] newLine = new int[4];
        int colIdx = 0;
        int idx = 0;

        while (idx < lines.Count)
        {
            int c1 = lines[idx].col;
            int value = lines[idx].value;

            if (idx + 1 < lines.Count && lines[idx + 1].value == value)
            {
                int c2 = lines[idx + 1].col;
                int mergedValue = value * 2;

                newLine[colIdx] = mergedValue;
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
                newLine[colIdx] = value;

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
            if (map[r, c] != newLine[c])
            {
                map[r, c] = newLine[c];
                changed = true;
            }
        }

        return changed;
    }

    private bool MoveLineUp(int c)
    {
        //0이 아닌 타일 저장
        List<(int row, int value)> lines = new List<(int row, int value)>();
        for (int r = 0; r < 4; r++)
        {
            int v = map[r, c];
            if (v != 0)
                lines.Add((r, v));
        }

        if (lines.Count == 0)
            return false;

        int[] newCol = new int[4];
        int rowIdx = 0;
        int idx = 0;

        while (idx < lines.Count)
        {
            int r1 = lines[idx].row;
            int value = lines[idx].value;

            int targetRow = rowIdx;

            if (idx + 1 < lines.Count && lines[idx + 1].value == value)
            {
                int r2 = lines[idx + 1].row;
                int mergedValue = value * 2;

                newCol[targetRow] = mergedValue;
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
                newCol[targetRow] = value;

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
            if (map[r, c] != newCol[r])
            {
                map[r, c] = newCol[r];
                changed = true;
            }
        }

        return changed;
    }

    private bool MoveLineDown(int c)
    {
        //0이 아닌 타일 저장
        List<(int row, int value)> lines = new List<(int row, int value)>();
        for (int r = 3; r >= 0; r--)
        {
            int v = map[r, c];
            if (v != 0)
                lines.Add((r, v));
        }

        if (lines.Count == 0)
            return false;

        int[] newCol = new int[4];
        int rowIdx = 3;
        int idx = 0;

        while (idx < lines.Count)
        {
            int r1 = lines[idx].row;
            int value = lines[idx].value;

            if (idx + 1 < lines.Count && lines[idx + 1].value == value)
            {
                int r2 = lines[idx + 1].row;
                int mergedValue = value * 2;

                newCol[rowIdx] = mergedValue;
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
                newCol[rowIdx] = value;

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
            if (map[r, c] != newCol[r])
            {
                map[r, c] = newCol[r];
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
            if (!tile.gameObject.activeSelf)
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

        tile.gameObject.SetActive(true);
        tile.RectTransform.anchoredPosition = targetPos.anchoredPosition;
        tile.SetValue(value);

        tileMap[r, c] = tile;
    }

    private void PlayTitleAnimation()
    {
        Array.Clear(hasMainTile, 0, hasMainTile.Length);
        TileUI[,] newTileMap = new TileUI[4, 4];

        Sequence seq = DOTween.Sequence();

        foreach (var move in tileMoves)
        {
            var tile = tileMap[move.startRow, move.startCol];
            if (tile == null)
                continue;

            int startIndex = move.startRow * 4 + move.startCol;
            int endIndex = move.endRow * 4 + move.endCol;

            var startPos = tilePos[startIndex].anchoredPosition;
            var endPos = tilePos[endIndex].anchoredPosition;

            tile.RectTransform.anchoredPosition = startPos;

            bool isSecondMerged = move.merged && hasMainTile[move.endRow, move.endCol];
            int endRow = move.endRow;
            int endCol = move.endCol;
            int value = move.value;

            Tween tween = tile.RectTransform
                .DOAnchorPos(endPos, 0.15f)
                .SetEase(Ease.OutQuad);

            if (isSecondMerged)
            {
                tween.OnComplete(() =>
                {
                    tile.gameObject.SetActive(false);
                    tile.SetValue(0);
                });
            }
            else
            {
                hasMainTile[endRow, endCol] = true;
                newTileMap[endRow, endCol] = tile;

                tween.OnComplete(() =>
                {
                    tile.SetValue(value);
                });
            }

            seq.Join(tween);
        }

        seq.OnComplete(() =>
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    tileMap[r, c] = newTileMap[r, c];
                }
            }
            SpawnRandomTile();
        });
    }
}