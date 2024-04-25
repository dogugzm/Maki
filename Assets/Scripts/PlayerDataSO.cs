using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] 
public class PlayerDataSO : ScriptableObject
{
    public float minSwipeDistance = 50f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float slowDownFactor = 0.1f;
    public float groundCheckRadius = 0.1f;
    public float dashMaxCounter = 2;
    public float speed = 2;
    public float jumpPower = 2;


    public LayerMask groundLayerMask;
    public GameObject throwablePrefab;


}