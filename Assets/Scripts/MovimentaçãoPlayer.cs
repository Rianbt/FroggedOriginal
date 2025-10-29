using UnityEngine;

public class MovimentaçãoPlayer : MonoBehaviour
{
    // Velocidade horizontal do jogador
    public float velocidade = 5f;

    // Força aplicada ao pular
    public float forcaPulo = 8f;
    public float forcaPuloMaxima = 16f;
    public float tempoMaximoPulo = 0.5f;

    // Configurações de gravidade para pulo mais rápido
    public float gravidadePulo = 2.5f;
    public float gravidadeQueda = 3f;
    public float gravidadeNormal = 1f;

    // Força do impulso lateral no pulo
    public float forcaImpulsoLateral = 15f;

    // Verificação se está no chão
    public Transform checarChao;
    public float raioChecagem = 0.2f;
    public LayerMask chaoLayer;

    // Componentes
    private Rigidbody2D rb;
    private Animator animator;
    private bool estaNoChao;
    private bool carregandoPulo;
    private float tempoPressionado;
    private bool estaPulando;
    private Vector3 escalaOriginal;
    private bool viradoParaDireita = true;
    private Vector3 spawnPosition; // Posição inicial do jogador

    // Parâmetros do Animator
    private readonly int MOVIMENTO_HORIZONTAL = Animator.StringToHash("MovimentoHorizontal");
    private readonly int ESTA_NO_CHAO = Animator.StringToHash("EstaNoChao");
    private readonly int ESTA_PULANDO = Animator.StringToHash("EstaPulando");
    private readonly int CARREGANDO_PULO = Animator.StringToHash("CarregandoPulo");

    void Start()
    {
        // Pega as referências dos componentes
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravidadeNormal;

        // Armazena a escala original do player
        escalaOriginal = transform.localScale;

        // Se não encontrou o Animator, tenta encontrar nos filhos
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Armazena a posição inicial do jogador
        spawnPosition = transform.position;
    }

    void Update()
    {
        // Verifica se está tocando o chão
        bool estavaNoChao = estaNoChao;
        estaNoChao = Physics2D.OverlapCircle(checarChao.position, raioChecagem, chaoLayer);

        // Atualiza o estado de pulo quando toca no chão
        if (estaNoChao && !estavaNoChao)
        {
            estaPulando = false;
        }

        // Atualiza os parâmetros do Animator
        AtualizarAnimator();

        // Controles de pulo
        ProcessarPulo();

        // Movimento horizontal
        ProcessarMovimento();
    }

    void ProcessarMovimento()
    {
        if (!carregandoPulo)
        {
            float movimento = 0f;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                movimento = 1f;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                movimento = -1f;
            }

            rb.linearVelocity = new Vector2(movimento * velocidade, rb.linearVelocity.y);

            if (movimento != 0)
            {
                VirarPersonagem(movimento > 0);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void ProcessarPulo()
    {
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && !carregandoPulo && !estaPulando)
        {
            IniciarCarregamentoPulo();
        }

        if (Input.GetKey(KeyCode.Space) && carregandoPulo)
        {
            tempoPressionado += Time.deltaTime;

            if (tempoPressionado >= tempoMaximoPulo)
            {
                ExecutarPulo();
                carregandoPulo = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && carregandoPulo)
        {
            ExecutarPulo();
            carregandoPulo = false;
        }

        if (!carregandoPulo)
        {
            if (rb.linearVelocity.y > 0 && !estaNoChao)
            {
                rb.gravityScale = gravidadePulo;
                estaPulando = true;
            }
            else if (rb.linearVelocity.y < 0 && !estaNoChao)
            {
                rb.gravityScale = gravidadeQueda;
                estaPulando = true;
            }
            else if (estaNoChao && Mathf.Abs(rb.linearVelocity.y) <= 0.1f)
            {
                rb.gravityScale = gravidadeNormal;
                estaPulando = false;
            }
        }
    }

    void AtualizarAnimator()
    {
        if (animator == null) return;

        float movimentoHorizontal = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat(MOVIMENTO_HORIZONTAL, movimentoHorizontal);
        animator.SetBool(ESTA_NO_CHAO, estaNoChao);
        animator.SetBool(ESTA_PULANDO, estaPulando);
        animator.SetBool(CARREGANDO_PULO, carregandoPulo);
    }

    void VirarPersonagem(bool paraDireita)
    {
        viradoParaDireita = paraDireita;

        Vector3 novaEscala = escalaOriginal;
        novaEscala.x = paraDireita ? Mathf.Abs(escalaOriginal.x) : -Mathf.Abs(escalaOriginal.x);
        transform.localScale = novaEscala;
    }

    void IniciarCarregamentoPulo()
    {
        carregandoPulo = true;
        estaPulando = false;
        tempoPressionado = 0f;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.gravityScale = 0f;
    }

    void ExecutarPulo()
    {
        rb.gravityScale = gravidadePulo;

        float forcaAtual = Mathf.Lerp(forcaPulo, forcaPuloMaxima, tempoPressionado / tempoMaximoPulo);

        float impulsoHorizontal = viradoParaDireita ? forcaImpulsoLateral : -forcaImpulsoLateral;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x + impulsoHorizontal, forcaAtual);

        estaPulando = true;
    }

    void OnDrawGizmosSelected()
    {
        if (checarChao == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checarChao.position, raioChecagem);
    }

    // Adicionando os métodos necessários para o script Coletaveis
    public void RecuperarVida()
    {
        Debug.Log("Vida recuperada!");
    }

    public void AtivarPiscar()
    {
        Debug.Log("Efeito de piscar ativado!");
    }

    public void VoltarAoSpawn()
    {
        // Reposiciona o jogador na posição inicial
        transform.position = spawnPosition;

        // Opcional: Reinicia a velocidade do jogador
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}