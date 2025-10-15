using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Pontuação do jogador")]
    public int score = 0; // Pontos atuais

    // Método público para adicionar pontos
    public void AdicionarPontos(int quantidade)
    {
        score += quantidade;
        Debug.Log($"Score atualizado: {score}");
    }

    // Método público para resetar a pontuação
    public void ResetarScore()
    {
        score = 0;
        Debug.Log("Score resetado.");
    }
}
    