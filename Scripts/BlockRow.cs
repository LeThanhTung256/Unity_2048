using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRow : MonoBehaviour
{
    // List các cells trên hàng
    public BlockCell[] cells { get; private set; }

    // Lấy các cell ngay từ ban đầu
    private void Awake()
    {
        cells = GetComponentsInChildren<BlockCell>();
    }
}
