using UnityEngine;

public class CenarioInfinito : MonoBehaviour
{

    public float velocidadeCenario;

    // Update is called once per frame
    void Update()
    {
        MovimentarCenario();
    }

    private void MovimentarCenario()
    {
        Vector2 deslocamento = new Vector2(Time.time * velocidadeCenario, 0);
        GetComponent<Renderer>().material.mainTextureOffset = deslocamento;
    }
}
