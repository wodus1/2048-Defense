using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

enum Direction { left, right, up, down }
enum InputState { Idle, Dragging}

public class Game2048Manager : MonoBehaviour
{
    [SerializeField] private List<TileUI> tiles = new List<TileUI>();
    private int[,] map = new int[4,4];
    private InputState inputState = InputState.Idle;
    private Vector2 startTouchPos;
    private float dragThreshold = 50f;
    
    public int[,] Map => map;
    
    void Awake()
    {
        InitTiles();
        SpawnRandomTile();
        SpawnRandomTile();
    }
    
    void Update()
    {
        switch (inputState)
        {
            case InputState.Idle:
                DetectDragStart();
                break;

            case InputState.Dragging:
                DetectDragEnd();
                break;
        }
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
    
    private void DetectDragStart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
            inputState = InputState.Dragging;
            return;
        }
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPos = Input.GetTouch(0).position;
            inputState = InputState.Dragging;
        }
    }

    private void DetectDragEnd()
    {
        Vector2 currentPos = Vector2.zero;
        
        if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
        }
        
        if (Input.touchCount > 0)
        {
            currentPos = Input.GetTouch(0).position;
        }
        
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
            inputState = InputState.Idle;
        }
        else
        {
            inputState = InputState.Idle;
        }
        
        inputState = InputState.Idle;
    }
    
    private bool MoveTile(Direction dir)
    {
        bool moved = false;

        if (dir == Direction.right)
        {
            
            for (int y = 0; y < 4; y++)
            {
                int[] line = new int[4];
                for (int x = 0; x < 4; x++)
                    line[x] = map[x, y];

                bool lineMoved = false;
                //int[] merged = MergLine(line, true, out lineMoved);

                if (lineMoved)
                    moved = true;

                /*for (int x = 0; x < 4; x++)
                    map[x, y] = merged[x];*/
            }

            return moved;
        }
        
        
        return moved;
    }

    /*int[] MergLine(int[] line, bool collapse, out bool moved)
    {
        int[] a = new int[line.Length];
        
        return a;
    }*/
    
    private void SpawnRandomTile()
    {
        List<(int x, int y)> empty = new List<(int x, int y)>();

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (map[x, y] == 0)
                    empty.Add((x, y));
            }
        }

        if (empty.Count == 0)
        {
            RefreshTiles();
            return;
        }
        
        var pos = empty[Random.Range(0, empty.Count)];
        
        float v = Random.Range(0f, 1f);
        map[pos.x, pos.y] = (v < 0.62f) ? 2 : 4;

        RefreshTiles();
    }
}
