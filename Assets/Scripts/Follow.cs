using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject target;
    public GameObject linkPrefab;
    public int numLinks = 3;

    List<GameObject> links;

    // Start is called before the first frame update
    void Start()
    {
        links = new List<GameObject>();
        for (int i=0; i<numLinks; i++)
        {
            // Instantiate the prefab
            GameObject link = Instantiate(linkPrefab, Vector3.zero, Quaternion.identity);
            links.Add(link);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MakeFollow(links[links.Count-1], target.transform.position, new Vector3(0, 0, 0));
        for (int i=(numLinks - 2); i>=1 ; i--)
        {
            MakeFollow(links[i], links[i + 1].transform.position, new Vector3(0, 0, 0));
        }

        MakeFollow(links[0], links[1].transform.position, new Vector3(0, 0, 0));

        // Ground offset
        links[0].transform.position = new Vector3(0, 0, 0);
        
        for (int i=1; i<numLinks; i++)
        {
            links[i].transform.position = links[i-1].transform.GetChild(0).GetChild(0).transform.position;
        }
    }

    void MakeFollow(GameObject link, Vector3 targetPosition, Vector3 freezeRotation)
    {
        Vector3 diff = targetPosition - link.transform.position;
        Vector3 heading = diff / Vector3.Magnitude(diff);

        // Rotation update
        Vector3 prevEulerAngles = link.transform.eulerAngles;
        link.transform.rotation = Quaternion.LookRotation(heading);
        Vector3 currentEulerAngles = link.transform.eulerAngles;

        if (freezeRotation.x == 1)
        {
            Debug.Log("Free X");
            currentEulerAngles.x = prevEulerAngles.x;
        }
        if (freezeRotation.y == 1)
            currentEulerAngles.y = prevEulerAngles.y;
        if (freezeRotation.z == 1)
            currentEulerAngles.z = prevEulerAngles.z;

        link.transform.eulerAngles = currentEulerAngles;

        // Position update
        link.transform.position = targetPosition + (link.transform.localScale.z * -heading);
        // MakeFollow()
    }
}
