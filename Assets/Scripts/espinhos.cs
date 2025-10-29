using UnityEngine;

public class espinhos : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obt�m o componente do jogador e reseta sua posi��o
            Movimenta��oPlayer player = collision.gameObject.GetComponent<Movimenta��oPlayer>();
            if (player != null)
            {
                player.VoltarAoSpawn();
            }
        }
    }
}