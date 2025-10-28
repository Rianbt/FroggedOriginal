using UnityEngine;

public class Movimentac3oPlayer : MonoBehaviour
{
    // Velocidade horizontal do jogador
    public float velocidade = 5f;

    // Força aplicada ao pular
    public float forcaPulo = 8f;
    public float forcaPuloMaxima = 12f;
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
        // SÓ PERMITE MOVIMENTO SE NÃO ESTIVER CARREGANDO PULO
        if (!carregandoPulo)
        {
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

            // Atualiza a direção do personagem
            if (movimento != 0)
            {
                VirarPersonagem(movimento > 0);
            }
        }
        else
        {
            // Se estiver carregando pulo, mantém a velocidade horizontal em zero
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void ProcessarPulo()
    {
        // Inicia o carregamento do pulo quando espaço é pressionado e está no chão
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && !carregandoPulo && !estaPulando)
        {
            IniciarCarregamentoPulo();
        }

        // Enquanto espaço estiver pressionado e estiver carregando o pulo
        if (Input.GetKey(KeyCode.Space) && carregandoPulo)
        {
            tempoPressionado += Time.deltaTime;

            // Limite máximo de carregamento
            if (tempoPressionado >= tempoMaximoPulo)
            {
                ExecutarPulo();
                carregandoPulo = false;
            }
        }

        // Executa o pulo quando espaço é solto
        if (Input.GetKeyUp(KeyCode.Space) && carregandoPulo)
        {
            ExecutarPulo();
            carregandoPulo = false;
        }

        // Controla a gravidade baseada no estado do pulo
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

        // Calcula movimento horizontal absoluto para animação
        float movimentoHorizontal = Mathf.Abs(rb.linearVelocity.x);

        // Atualiza todos os parâmetros do Animator
        animator.SetFloat(MOVIMENTO_HORIZONTAL, movimentoHorizontal);
        animator.SetBool(ESTA_NO_CHAO, estaNoChao);
        animator.SetBool(ESTA_PULANDO, estaPulando);
        animator.SetBool(CARREGANDO_PULO, carregandoPulo);
    }

    void VirarPersonagem(bool paraDireita)
    {
        viradoParaDireita = paraDireita;

        Vector3 novaEscala = escalaOriginal;
        if (!viradoParaDireita)
        {
            novaEscala.x = -Mathf.Abs(escalaOriginal.x);
        }
        else
        {
            novaEscala.x = Mathf.Abs(escalaOriginal.x);
        }
        transform.localScale = novaEscala;
    }

    void IniciarCarregamentoPulo()
    {
        carregandoPulo = true;
        estaPulando = false;
        tempoPressionado = 0f;

        // Para o movimento vertical e congela no eixo Y
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.gravityScale = 0f;
    }

    void ExecutarPulo()
    {
        // Restaura a gravidade
        rb.gravityScale = gravidadePulo;

        // Calcula força do pulo baseada no tempo pressionado
        float forcaAtual = Mathf.Lerp(forcaPulo, forcaPuloMaxima, tempoPressionado / tempoMaximoPulo);

        // Adiciona um impulso horizontal na direção que o personagem está virado
        float impulsoHorizontal = viradoParaDireita ? forcaImpulsoLateral : -forcaImpulsoLateral;

        // Aplica o pulo - MANTÉM a velocidade horizontal atual e ADICIONA o impulso
        Vector2 velocidadeAtual = rb.linearVelocity;
        rb.linearVelocity = new Vector2(velocidadeAtual.x + impulsoHorizontal, forcaAtual);

        estaPulando = true;
    }

    void OnDrawGizmosSelected()
    {
        if (checarChao == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checarChao.position, raioChecagem);
    }
}