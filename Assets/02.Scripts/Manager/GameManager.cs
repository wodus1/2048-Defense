using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

enum Direction { left, right, up, down }
enum InputState { Idle, Dragging }

public class GameManager : MonoBehaviour //게임 매니저(2048 로직)
{
    [SerializeField] private SubSystemsManager subSystemsManager;
    [SerializeField] private List<TileUI> tiles = new List<TileUI>();
    private int[,] map = new int[4,4];
    private InputState inputState = InputState.Idle;
    private Vector2 startTouchPos;
    private float dragThreshold = 50f;

    public List<TileUI> Tiles => tiles;
    public SubSystemsManager SubSystemsManager => subSystemsManager;

    void Awake()
    {
        InitGameSetting();
    }
    
    void Update()
    {
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

    private void InitGameSetting()
    {
        InitTiles();
        subSystemsManager.Initialize(this);
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
        int index = 0;

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                tiles[index].SetValue(map[r, c]);
                index++;
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
            SpawnRandomTile();
        }
        
        inputState = InputState.Idle;
    }
    
    private bool MoveTile(Direction dir)
    {
        bool moved = false;

        switch (dir)
        {
            case Direction.left:
                moved = LeftMove();
                Debug.Log("왼쪽");
                break;
            case Direction.right:
                moved = RightMove();
                Debug.Log("오른쪽");
                break;
            case Direction.up:
                moved = UpMove();
                Debug.Log("위쪽");
                break;
            case Direction.down:
                moved = DownMove();
                Debug.Log("아래쪽");
                break;
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
            RefreshTiles();
            return;
        }
        
        var pos = empty[Random.Range(0, empty.Count)];
        
        float v = Random.Range(0f, 1f);
        map[pos.r, pos.c] = (v < 0.55f) ? 2 : 4;

        RefreshTiles();
    }
    
    
    #region 타일 이동
    private bool RightMove()
    {
        bool moved = false;
        
        for (int r = 0; r < 4; r++)
        {
            List<int> lines = new List<int>();
            for (int c = 3; c >= 0; c--)
            {
                if(map[r,c] != 0)
                    lines.Add(map[r,c]);
            }
                
            List<int> merge = MergeLine(lines);

            int mergeIdx = 0;
            for (int c = 3; c >= 0; c--)
            {
                int newValue = (mergeIdx < merge.Count) ? merge[mergeIdx] : 0;
                    
                if (map[r, c] != newValue)
                {
                    moved = true;
                    map[r, c] = newValue;
                }

                mergeIdx++;
            }
        }
        
        return moved;
    }
    
    private bool LeftMove()
    {
        bool moved = false;
        
        for (int r = 0; r < 4; r++)
        {
            List<int> lines = new List<int>();
            for (int c = 0; c < 4; c++)
            {
                if(map[r,c] != 0)
                    lines.Add(map[r,c]);
            }
                
            List<int> merge = MergeLine(lines);

            int mergeIdx = 0;
            for (int c = 0; c < 4; c++)
            {
                int newValue = (mergeIdx < merge.Count) ? merge[mergeIdx] : 0;
                    
                if (map[r, c] != newValue)
                {
                    moved = true;
                    map[r, c] = newValue;
                }

                mergeIdx++;
            }
        }
        
        return moved;
    }
    
    private bool UpMove()
    {
        bool moved = false;
        
        for (int c = 0; c < 4; c++)
        {
            List<int> lines = new List<int>();
            for (int r = 0; r < 4; r++)
            {
                if(map[r,c] != 0)
                    lines.Add(map[r,c]);
            }
                
            List<int> merge = MergeLine(lines);
            
            int mergeIdx = 0;
            for (int r = 0; r < 4; r++)
            {
                int newValue = (mergeIdx < merge.Count) ? merge[mergeIdx] : 0;
                    
                if (map[r, c] != newValue)
                {
                    moved = true;
                    map[r, c] = newValue;
                }

                mergeIdx++;
            }
        }
        
        return moved;
    }
    
    private bool DownMove()
    {
        bool moved = false;
        
        for (int c = 0; c < 4; c++)
        {
            List<int> lines = new List<int>();
            for (int r = 3; r >= 0; r--)
            {
                if(map[r,c] != 0)
                    lines.Add(map[r,c]);
            }
                
            List<int> merge = MergeLine(lines);
            
            int mergeIdx = 0;
            for (int r = 3; r >= 0; r--)
            {
                int newValue = (mergeIdx < merge.Count) ? merge[mergeIdx] : 0;
                    
                if (map[r, c] != newValue)
                {
                    moved = true;
                    map[r, c] = newValue;
                }

                mergeIdx++;
            }
        }
        
        return moved;
    }
    
    private List<int> MergeLine(List<int> line)
    {
        List<int> merged = new List<int>();
        int idx = 0;

        while (idx < line.Count)
        {
            if (idx + 1 < line.Count && line[idx] == line[idx + 1])
            {
                merged.Add(line[idx] * 2);
                idx += 2;
            }
            else
            {
                merged.Add(line[idx]);
                idx += 1;
            }
        }

        return merged;
    }
    #endregion
}