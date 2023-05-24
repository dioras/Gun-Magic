using System.Collections;
using _GAME.Common;
using DG.Tweening;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("ScaleMe")]
    private void Scale()
    {
        GetComponent<Ragdoll>().ActivateRagdoll();
        transform.DOScale(0.1f, 1.5f);
    }

    [ContextMenu("ScaleMeCoroutine")]
    private void ScaleByCorotine()
    {
        GetComponent<Ragdoll>().ActivateRagdoll();
        StartCoroutine(ScaleOverSeconds(gameObject, new Vector3(0.1f, 0.1f, 0.1f), 1.5f));
    }

    public IEnumerator ScaleOverSeconds(GameObject objectToScale, Vector3 scaleTo, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingScale = objectToScale.transform.localScale;
        while (elapsedTime < seconds)
        {
            objectToScale.transform.localScale = Vector3.Lerp(startingScale, scaleTo, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToScale.transform.position = scaleTo;
    }
}
