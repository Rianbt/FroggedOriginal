using UnityEngine;

public class PlayerBasico : MonoBehaviour
{
    // Velocidade horizontal do jogador
    public float velocidade = 5f;

    // Força aplicada ao pular
    public float forcaPulo = 8f;

    // Verificação se está no chão
    public Transform checarChao;
    public float raioChecagem = 0.2f;
    public LayerMask chaoLayer;
        
    // Componentes
    private Rigidbody2D rb;
    private bool estaNoChao;

    void Start()
    {
        // Pega a referência ao Rigidbody2D do jogador
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Captura o movimento horizontal (setas ou A/D)
        float movimento = Input.GetAxisRaw("Horizontal");

        // Define a velocidade horizontal (mantém a velocidade vertical)
        rb.linearVelocity = new Vector2(movimento * velocidade, rb.linearVelocity.y);

        // Verifica se está tocando o chão (para permitir pular)
        estaNoChao = Physics2D.OverlapCircle(checarChao.position, raioChecagem, chaoLayer);

        // Se o jogador apertar espaço e estiver no chão, pula
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        // Inverte o sprite quando muda de direção (opcional)
        if (movimento != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movimento), 1f, 1f);
        }
    }

    // Desenha o círculo de checagem do chão no editor (ajuda visualmente)
    void OnDrawGizmosSelected()
    {
        if (checarChao == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checarChao.position, raioChecagem);
    }
}
