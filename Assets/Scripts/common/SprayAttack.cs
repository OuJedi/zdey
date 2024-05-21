using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayAttack : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("SprayAttack:" + collision.gameObject.tag);

        if (collision.gameObject.tag == "Pigeon")
        {
            collision.gameObject.GetComponent<Pigeon>().Fall();
        }
    }

    public void onSprayAttackClipComplete(string name)
    {

        switch (name)
        {

            case "end":
                Destroy(gameObject);
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
