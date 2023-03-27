// Xử lý tương tác game - Quản lý chung cho game play
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Block blockPrefab;
    public BlockState[] states;
    private InputManager inputManager;
    private BlockGrid grid;

    private AudioSource mergeSound;

    // Danh sách các block đang có
    public List<Block> blocks;
    public List<Block> reuseBlocks;
    
    // isWaiting dùng để ngăn chặn người dùng nhập vào input trước khi animate kết thúc
    private bool isWaiting;

    private void Awake()
    {
        grid = GetComponentInChildren<BlockGrid>();
        mergeSound = GetComponent<AudioSource>();
        inputManager = GetComponent<InputManager>();
        blocks = new List<Block>(16);
        reuseBlocks = new List<Block>(16);
    } 

    private void Update()
    {
        if (!isWaiting)
        {
            Vector2Int direction = inputManager.GetDirection();
            if (direction!= Vector2Int.zero)
            {
                if (direction == Vector2Int.up)
                {
                    MoveBlocks(direction, 0, 1, 1, 1);
                }
                else if (direction == Vector2Int.left)
                {
                    MoveBlocks(direction, 1, 0, 1, 1);
                }
                else if (direction == Vector2Int.down)
                {
                    MoveBlocks(direction, 0, grid.height - 2, 1, -1);
                }
                else if (direction == Vector2Int.right)
                {
                    MoveBlocks(direction, grid.height - 2, 0, -1, 1);
                }

                inputManager.ResetDirection();
            }
        }
    }

    // Xoá hết các block có trên bảng
    public void ClearBoard()
    {
        foreach(Block block in blocks)
        {
            block.Reset();
            reuseBlocks.Add(block);
        }

        blocks.Clear();
    }

    // Tạo một block mới tại vị trí cell trống
    public void CreateBlock()
    {
        BlockCell emptyCell = grid.GetRandomEmptyCell();

        // Nếu có block có thể tái sử dụng thì dùng, nếu không thì tạo mới
        if (reuseBlocks.Count != 0)
        {
            Block reuseBlock = reuseBlocks[0];
            reuseBlock.Reuse(emptyCell);
            blocks.Add(reuseBlock);
            reuseBlocks.Remove(reuseBlock);
        }
        else 
        {
            Block newBlock = Instantiate(blockPrefab, grid.transform);
            if (emptyCell != null)
            {
                newBlock.SetState(states[0], 2);
                newBlock.Spawn(emptyCell);
            }
            blocks.Add(newBlock);       
        }
 
    }

    // Di chuyển tất cả block theo hướng direction
    private void MoveBlocks(Vector2Int direction,int startX, int startY,int incrementX, int incrementY)
    {
        bool changed = false;

        // Cho phép cho tất cả các block
        foreach(Block block in blocks)
        {
            block.SetMergeState(true);
        }
        
        for (int x = startX; x >= 0 && x < grid.width; x+=incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y+=incrementY)
            {
                // Nếu tại vị trí này có block thì di chuyển block
                BlockCell cell = grid.GetCell(x, y);
                if (!cell.isEmpty)
                {
                    changed |= MoveBlock(cell.block, direction);
                }
            }
        }
        if (changed)
        {
            mergeSound.Play();
            StartCoroutine(WaitForChanges());
        }
    }

    // Di chuyển một khối theo hướng chỉ định
    private bool MoveBlock(Block block, Vector2Int direction)
    {
        BlockCell newCell = null;
        BlockCell adjacentCell = grid.GetAdjacentCell(block.cell, direction);

        while (adjacentCell != null)
        {
            // Nếu adjacentCell có block thì có thể merge
            if (!adjacentCell.isEmpty)
            {
                // Nếu block và adjacentCell.block giống nhau thì merge
                // Nếu không block sẽ di chuyển tới newCell.
                if (CanMerge(block, adjacentCell) && adjacentCell.block.mergeState)
                {
                    // Remove block khỏi danh sách
                    blocks.Remove(block);

                    // Merge block into adjacent block
                    block.MergeInto(adjacentCell.block);

                    reuseBlocks.Add(block);

                    // Tăng indexState lên 1, nếu nó vượt quá states.length thì lấy sates.length
                    int indexState = Mathf.Clamp(IndexOfState(adjacentCell.block.state) + 1, 0, states.Length - 1);

                    // Nếu state thay đổi thì number nhân 2, nếu không thì không thay đổi
                    int number = (indexState == IndexOfState(adjacentCell.block.state)) ? adjacentCell.block.number : adjacentCell.block.number * 2;

                    // Nếu có ô đạt được 2048 thì thông báo won game
                    if (number >= 2048)
                    {
                        bool isWon = Convert.ToBoolean(PlayerPrefs.GetInt("isWon", 0));
                        if(!isWon)
                        {
                            gameManager.GameWon();
                        }
                    }
                    
                    adjacentCell.block.SetState(states[indexState], number);

                    // Nếu trong lượt này này block đã merge thì không cho merge nữa
                    adjacentCell.block.SetMergeState(false);

                    // Tăng điểm cho trò chơi
                    gameManager.IncreaseScore(number);

                    return true;
                }

                break;
            }

            newCell = adjacentCell;
            adjacentCell = grid.GetAdjacentCell(newCell, direction);

        }

        // Nếu newCell != null, tức là newCell là cái ngoài cùng. Di chuyển block đến đó
        if (newCell != null)
        {
            block.MoveTo(newCell);
            return true;
        }

        return false;
    }

    // Kiểm tra block có thể merge vào cell.block được hay không
    private bool CanMerge(Block block, BlockCell cell)
    {
        return (block.number == cell.block.number);
    }

    // Trả về index của một state
    public int IndexOfState(BlockState state)
    {
        return Array.IndexOf(states, state);
    }

    public BlockState GetState(string name)
    {
        foreach(BlockState state in states)
        {
            if (state.name == name)
            { return state; }
        }

        return null;
    }

    // Đợi kết thúc animate
    private IEnumerator WaitForChanges()
    {
        isWaiting = true;

        yield return new WaitForSeconds(0.1f);

        isWaiting = false;

        if (blocks.Count <= grid.size)
        {
            CreateBlock();
        }

        // Kiểm tra xem game over chưa
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }    
    }


    // Kiểm tra game đã over chưa
    private bool CheckForGameOver()
    {
        // Nếu số lượng blocks chưa đủ thì return false
        if (blocks.Count != grid.size)
        {
            return false;
        }

        // Kiểm tra tất cả các ô xem còn có thể merge được hay không, nếu có thì return false
        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.width; x++)
            {
                // Lấy cell tại vị trí (x, y)
                BlockCell cell = grid.GetCell(x, y);

                // Nếu cell không phải ở cột cuối cùng, kiểm tra xem có thể merge vào ô bên phải hay không
                if (x < grid.width - 1)
                {
                    BlockCell right = grid.GetAdjacentCell(cell, Vector2Int.right);
                    if (CanMerge(cell.block, right))
                    {
                        return false;
                    }
                }
                // Nếu cell không phải ở hàng cuối cùng, kiểm tra xem có thể merge vào ô bên dưới hay không
                if (y < grid.height - 1)
                {        
                    BlockCell down = grid.GetAdjacentCell(cell, Vector2Int.down);
                    if (CanMerge(cell.block, down))
                    {
                        return false;
                    }    
                }
            }
        }

        return true;
    }

    public void LoadGame(List<Vector2Int> listCoor, List<int> listNum, List<BlockState> listState)
    {
        for (int i = 0; i < listCoor.Count; i++)
        {
            BlockCell cell = grid.GetCell(listCoor[i]);
            Block newBlock = Instantiate(blockPrefab, grid.transform);
            newBlock.SetState(listState[i], listNum[i]);
            newBlock.Spawn(cell);

            blocks.Add(newBlock);
        }
    }

    public bool LoadGameState()
    {
        if (!PlayerPrefs.HasKey("blocksSize"))
        {
            return false;
        }

        int blocksSize = PlayerPrefs.GetInt("blocksSize", 0);
        if (blocksSize == 0) return false;

        List<Vector2Int> listCoor = new List<Vector2Int>(blocksSize);
        List<int> listNum = new List<int>(blocksSize);
        List<BlockState> listState = new List<BlockState>(blocksSize);

        foreach (string coor in PlayerPrefs.GetString("coor").Split("|"))
        {
            string strVector = coor;
            int x = int.Parse(strVector.Split(",")[0].Trim('('));
            int y = int.Parse(strVector.Split(",")[1].Trim(')'));
            listCoor.Add(new Vector2Int(x, y));
        }

        foreach (string num in PlayerPrefs.GetString("number").Split("|"))
        {
            listNum.Add(int.Parse(num));
            listState.Add(GetState(num));
        }

        LoadGame(listCoor, listNum, listState);

        return true;
    }
}
