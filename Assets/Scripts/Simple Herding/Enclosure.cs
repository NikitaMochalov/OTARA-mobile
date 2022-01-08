using System.Collections;
using TMPro;
using UnityEngine;

public class Enclosure : MonoBehaviour
{
    public int scoresPerSheep = 1;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            GameManager.Instance.CaughtSheepsCount++;
            GameManager.Instance.AddScores(scoresPerSheep);

            StartCoroutine(HideSheep(go));
        }
    }

    IEnumerator HideSheep(GameObject sheepObject)
    {
        sheepObject.layer = LayerMask.NameToLayer("Default");

        yield return new WaitForSeconds(1);

        sheepObject.GetComponent<SheepController>().enabled = false;
        sheepObject.GetComponent<Graze>().enabled = false;
        sheepObject.GetComponent<Fright>().enabled = false;

        yield return new WaitForSeconds(1);

        FindObjectOfType<Dog>().sheepsAround.Remove(sheepObject.GetComponent<SheepController>());
        //Destroy(sheepObject);
    }
}
