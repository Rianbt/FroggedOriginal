using UnityEngine;

public class PlayerBasico : MonoBehaviour
{
    // Velocidade horizontal do jogador
    public float velocidade = 5f;

    // Força aplicada ao pular
    public float forcaPulo = 8f;
    public float forcaPuloMaxima = 12f;
    public float tempoMaximoPulo = 0.5f;

    // Configurações de gravidade para pulo mais rápido
    public float gravidadePulo = 2.5f;    // Gravidade durante o pulo (subida)
    public float gravidadeQueda = 3f;     // Gravidade durante a queda
    public float gravidadeNormal = 1f;    // Gravidade normal

    // Verificação se está no chão
    public Transform checarChao;
    public float raioChecagem = 0.2f;
    public LayerMask chaoLayer;

    // Componentes
    private Rigidbody2D rb;
    private bool estaNoChao;
    private bool carregandoPulo;
    private float tempoPressionado;
    private bool estaPulando;

    void Start()
    {
        // Pega a referência ao Rigidbody2D do jogador
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravidadeNormal;
        Debug.Log("Player iniciado - Rigidbody: " + (rb != null));
    }

    void Update()
    {
        // Verifica se está tocando o chão (para permitir pular)
        bool estavaNoChao = estaNoChao;
        estaNoChao = Physics2D.OverlapCircle(checarChao.position, raioChecagem, chaoLayer);

        // DEBUG: Mostra estado atual (pode remover depois que estiver funcionando)
        //Debug.Log($"Estado: noChao={estaNoChao}, carregando={carregandoPulo}, pulando={estaPulando}, velY={rb.linearVelocity.y}");

        // Inicia o carregamento do pulo quando espaço é pressionado e está no chão
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && !carregandoPulo && !estaPulando)
        {
            Debug.Log("Iniciando carregamento do pulo");
            IniciarCarregamentoPulo();
        }

        // Enquanto espaço estiver pressionado e estiver carregando o pulo
        if (Input.GetKey(KeyCode.Space) && carregandoPulo)
        {
            tempoPressionado += Time.deltaTime;
            //Debug.Log($"Carregando pulo: {tempoPressionado:F2}s");

            // Limite máximo de carregamento
            if (tempoPressionado >= tempoMaximoPulo)
            {
                Debug.Log("Pulo automático (tempo máximo)");
                ExecutarPulo();
                carregandoPulo = false;
            }
        }

        // Executa o pulo quando espaço é solto
        if (Input.GetKeyUp(KeyCode.Space) && carregandoPulo)
        {
            Debug.Log("Pulo executado (tecla solta)");
            ExecutarPulo();
            carregandoPulo = false;
        }

        // Controla a gravidade baseada no estado do pulo
        if (!carregandoPulo)
        {
            if (rb.linearVelocity.y > 0 && !estaNoChao)
            {
                // Subindo - aplica gravidade mais forte para subida rápida
                rb.gravityScale = gravidadePulo;
                estaPulando = true;
            }
            else if (rb.linearVelocity.y < 0 && !estaNoChao)
            {
                // Caindo - aplica gravidade ainda mais forte para queda rápida
                rb.gravityScale = gravidadeQueda;
                estaPulando = false;
            }
            else if (estaNoChao && rb.linearVelocity.y <= 0.1f)
            {
                // No chão - gravidade normal
                rb.gravityScale = gravidadeNormal;
                estaPulando = false;

                // Reseta o estado de carregamento apenas quando realmente está no chão
                if (carregandoPulo)
                {
                    Debug.Log("Resetando carregamento no chão");
                    carregandoPulo = false;
                }
            }
        }

        // Movimento horizontal
        float movimento = 0f;

        // Tecla para direita (D ou seta direita)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movimento = 1f;
        }
        // Tecla para esquerda (A ou seta esquerda)
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movimento = -1f;
        }

        // Define a velocidade horizontal (mantém a velocidade vertical)
        rb.linearVelocity = new Vector2(movimento * velocidade, rb.linearVelocity.y);

        // Inverte o sprite quando muda de direção
        if (movimento != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movimento), 1f, 1f);
        }
    }

    void IniciarCarregamentoPulo()
    {
        carregandoPulo = true;
        estaPulando = false;
        tempoPressionado = 0f;

        // Para o movimento vertical e congela no eixo Y
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.gravityScale = 0f; // Remove a gravidade enquanto carrega

        Debug.Log("Carregamento iniciado - gravidade: 0");
    }

    void ExecutarPulo()
    {
        // Restaura a gravidade
        rb.gravityScale = gravidadePulo;

        // Calcula força do pulo baseada no tempo pressionado
        float forcaAtual = Mathf.Lerp(forcaPulo, forcaPuloMaxima, tempoPressionado / tempoMaximoPulo);

        // Aplica o pulo
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaAtual);
        estaPulando = true;

        Debug.Log($"Pulo executado! Força: {forcaAtual}, Tempo pressionado: {tempoPressionado:F2}, Gravidade: {rb.gravityScale}");
    }

    // Desenha o círculo de checagem do chão no editor (ajuda visualmente)
    void OnDrawGizmosSelected()
    {
        if (checarChao == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checarChao.position, raioChecagem);
    }
}