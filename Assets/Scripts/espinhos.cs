using UnityEngine;

public class espinhos : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtém o componente do jogador e reseta sua posição
            MovimentaçãoPlayer player = collision.gameObject.GetComponent<MovimentaçãoPlayer>();
            if (player != null)
            {
                player.VoltarAoSpawn();
            }
        }
    }
}