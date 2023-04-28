using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
}
