using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    // Tất cả các hàng trên grid
    public BlockRow[] rows { get; private set; }

    // Tất cả các cell trên grid
    public BlockCell[] cells { get; private set; }

    // Kích thước của grid
    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size / height;

    private void Awake()
    {
        // Lấy danh sách các rows và cells
        rows = GetComponentsInChildren<BlockRow>();
        cells = GetComponentsInChildren<BlockCell>();
    }

    private void Start()
    {
        // Thêm toạ độ (coordinates) cho tất các cells
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    // Lấy ngẫu nhiên một ô còn trống
    public BlockCell GetRandomEmptyCell()
    {
        // Lấy ngẫu nhiên một số từ 0 đến size của cells
        int index = Random.Range(0, size);
        // Lưu giá trị khởi tạo ban đầu
        int startIndex = index;

        while (!cells[index].isEmpty)
        {
            // Nếu cell vừa tìm được không còn trống thì tăng index lên 1
            index++;

            // Nếu index lớn hơn hoặc bằng size thì gán index về 0 và tiếp tục vòng lặp while
            if (index >= size)
            {
                index = 0;
            }

            // Nếu index bằng startIndex tức là không có ô cell nào còn trống, return null
            if (index == startIndex)
            {
                return null;
            }    
        }    

        return cells[index];
    }

    // Lấy cell theo vị trí hàng và cột
    public BlockCell GetCell(int col, int row)
    {
        if (row >= 0 && row < height && col >= 0 && col < width)
        {
            return rows[row].cells[col];
        }

        return null;
    }

    // Lấy cell theo toạ độ
    public BlockCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    // Lấy cell liền kề theo hướng chỉ định
    public BlockCell GetAdjacentCell(BlockCell cell, Vector2Int direction)
    {
        return GetCell(cell.coordinates.x + direction.x, cell.coordinates.y - direction.y);
    }    
}
