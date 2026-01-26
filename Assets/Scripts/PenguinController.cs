using Unity.VisualScripting;
using UnityEngine;

public class PenguinController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask terrainLayer;

    [SerializeField] Rigidbody penguinRB;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Awake()
    {
        penguinRB = this.GetComponent<Rigidbody>(); 
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 castPos = transform.position;

        castPos.y += 1;

        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDistance;
                transform.position = movePos; 
            }

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            Vector3 moveDir = new Vector3(x, 0, y);
            penguinRB.angularVelocity = moveDir * speed; 

            if (x != 0 && x < 0)
            {
                spriteRenderer.flipX = true;
            } else if (x != 0 && x > 0)
            {
                spriteRenderer.flipX = false; 
            }
        }
   }
}
