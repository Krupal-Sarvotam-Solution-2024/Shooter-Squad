using UnityEngine;

public class Powerups : MonoBehaviour
{
    public enum allpowerusp
    {
        shild,
        bomb,
    }
    public GameManager manager;
    public allpowerusp powerups;
    public float health;
    public float damgeradious;
    public void OnTriggerEnter(Collider other)
    {
        if (powerups == allpowerusp.shild)
        {
            if (other.GetComponent<Entity>())
            {

                other.GetComponent<Entity>().StartCoroutine(other.GetComponent<Entity>().shildAcitavte());
                this.gameObject.SetActive(false);
            }
          


        }
        else if(powerups == allpowerusp.bomb)
        {
            if (other.GetComponent<Bullet>())
            {
                health--;
                if(health<=0)
                    Blast();
            }
         


        }
    }
    public GameObject effectobject;
    public void Blast()
    {
        //activating the bomb effect
        GetComponent<MeshRenderer>().enabled = false;
        effectobject.SetActive(true);

        //get the srounding player


        for (int i = 0; i < manager.botAll.Count; i++)
        {
            if (Vector3.Distance(manager.botAll[i].transform.position, transform.position) < damgeradious)
            {
                manager.botAll[i].ReduceHeath(100);
            }


        }
        


    }
}


