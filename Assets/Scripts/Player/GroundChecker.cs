using System;
using UnityEngine;

/// <summary>
/// 地面检测器 - 多射线/盒检测
/// </summary>
public class GroundChecker : MonoBehaviour
{
    [Header("检测参数")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private Vector2 boxSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private int rayCount = 3;
    
    [Header("调试")]
    [SerializeField] private bool showDebug = true;
    
    private bool isGrounded;

    private void Update(){
        CheckGrounded();
    }

    /// <summary>
    /// 检测是否在地面
    /// </summary>
    public bool CheckGrounded()
    {
        Vector2 origin = transform.position;
        
        // 盒检测
        bool boxHit = Physics2D.OverlapBox(origin, boxSize, 0f, groundLayers);
        
        // 多射线检测
        bool rayHit = false;
        for (int i = 0; i < rayCount; i++)
        {
            float x = Mathf.Lerp(-boxSize.x / 2, boxSize.x / 2, (float)i / (rayCount - 1));
            Vector2 rayOrigin = origin + new Vector2(x, 0);
            
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, checkDistance, groundLayers);
            if (hit.collider != null)
            {
                rayHit = true;
                break;
            }
        }
        
        isGrounded = boxHit || rayHit;
        // Debug.Log(isGrounded);
        return isGrounded;
    }
    
    public bool IsGrounded => isGrounded;
    
    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;
        
        Vector2 origin = transform.position;
        
        // 绘制检测盒
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
        
        // 绘制射线
        Gizmos.color = Color.blue;
        for (int i = 0; i < rayCount; i++)
        {
            float x = Mathf.Lerp(-boxSize.x / 2, boxSize.x / 2, (float)i / (rayCount - 1));
            Vector2 rayOrigin = origin + new Vector2(x, 0);
            Gizmos.DrawRay(rayOrigin, Vector2.down * checkDistance);
        }
    }
}