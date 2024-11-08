using UnityEngine;

public class SpawnL2 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ob1; 
    public GameObject ob2;

    // Update is called once per frame
    void Awake()
    {
        for(int i = 0; i<100; i++){
            Vector3 randSpawn = new Vector3(Random.Range(20, 500), 1, Random.Range(20, 500));
            Vector3 randSpawn2 = new Vector3(Random.Range(20, 500), 1, Random.Range(20, 500));

            Instantiate(ob1, randSpawn, Quaternion.identity);
            Instantiate(ob2, randSpawn2, Quaternion.identity);

        }
    }
}
