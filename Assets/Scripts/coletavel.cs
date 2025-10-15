using UnityEngine;

public class Moeda : MonoBehaviour
{
    public int quantidade = 1; // Pontos da moeda
    public GameManager gameManager; // Referência ao GameManager

    void OnTriggerEnter2D(Collider2D jogador)
    {
        if (jogador.CompareTag("Player"))
        {
            // Exemplo: adicionar pontos ao jogador
            gameManager.AdicionarPontos(quantidade);

            // Destrói a moeda
            Destroy(gameObject);
        }
    }
}
