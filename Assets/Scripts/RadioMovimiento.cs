using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioMovimiento : MonoBehaviour
{
    // esta clase hereda de Unit porque no requiere ninguna de su funcionalidad

    public Transform _origenLadrido;
    public float _radioSonido = 10.0f;
    public LayerMask _mascaraEnemigos;

    private ConoDeVision _conoVision;

    public AudioSource _woofAudio;
    public bool _hasBarked = false;

    void Start()
    {
        _conoVision = GetComponentInChildren<ConoDeVision>();
    }

    void Update()
    {
        if (_conoVision.detected)
        {
            // cambia estado a alarma
            // Debug.Log("WOOF WOOF");
            if (!_hasBarked)
            {
                _hasBarked = true;
                _woofAudio.Play();
            }

            Ladrar();
        }
        else if (!_conoVision.detected)
        {
            // cambia estado a idle
            // Debug.Log("ZzZzz");
            _hasBarked = false;
            _woofAudio.Stop();

        }
    }

    private void Ladrar()
    {

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, _radioSonido, _mascaraEnemigos);

        foreach (Collider col in colliders)
        {
            GameObject cultistaActual = col.gameObject;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, _radioSonido);
    }
}
