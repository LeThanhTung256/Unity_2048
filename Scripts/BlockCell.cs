using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCell : MonoBehaviour
{
    // Toạ độ của cell
    public Vector2Int coordinates { get; set; }

    // Block nằm trên cell
    public Block block { get; set; }

    // Kiểm tra block có còn trống không
    public bool isEmpty => block == null;
}
