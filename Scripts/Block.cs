using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Block : MonoBehaviour
{
    // Color của Block (background, text)
    public BlockState state { get; private set; }

    // Vị trí (cell) của Block
    public BlockCell cell { get; private set; }

    // Số trên block
    public int number { get; private set; }

    // Liệu block có thể merge
    public bool mergeState { get; private set; }

    // Các component của Block
    private Image background;
    private TextMeshProUGUI text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        this.cell = null;
        Destroy(gameObject);
    }

    // Thiết lập thuộc tính cho block
    public void SetState(BlockState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    // Set merge state cho block
    public void SetMergeState(bool isAllow)
    {
        this.mergeState = isAllow;
    }    

    // Sinh ra tại vị trí cell
    public void Spawn(BlockCell cell)
    {
        if (this.cell != null)
        {
            this.cell.block = null;
        }

        this.cell = cell;
        this.cell.block = this;

        transform.position = this.cell.transform.position;
    }

    // Di chuyển block tới vị trí của cell
    public void MoveTo(BlockCell cell)
    {
        if (this.cell != null)
        {
            this.cell.block = null;
        }

        this.cell = cell;
        this.cell.block = this;

        StartCoroutine(Animate(cell.transform.position));
    }

    // Merge block vào block khác
    public void MergeInto(Block block)
    {
        if (this.cell != null)
        {
            this.cell.block = null;
        }

        StartCoroutine(Animate(block.transform.position));
        Destroy(gameObject);
    }

    // Tạo animate di chuyển
    public IEnumerator Animate(Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(transform.position, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;  
        }

        transform.position = to;
    }
}
