using UnityEngine;

public class Coletaveis : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool coletado = false;

    public Material materialBrilho;
    public GameObject efeitoColetaPrefab;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !coletado)
        {
            coletado = true;

            var player = collision.GetComponent<MovimentaçãoPlayer>();
            if (player != null)
            {
                player.RecuperarVida();
                player.AtivarPiscar();
            }

            if (animator != null)
            {
                animator.SetTrigger("Coletado");
            }

            if (spriteRenderer != null && materialBrilho != null)
            {
                spriteRenderer.material = materialBrilho;
            }

            if (efeitoColetaPrefab != null)
            {
                Instantiate(efeitoColetaPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject, 0.5f);
        }
    }
}