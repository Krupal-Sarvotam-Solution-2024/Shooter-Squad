using UnityEngine;

public class Powerups : MonoBehaviour
{
    public enum allpowerusp
    {
        shild,
        bomb,
        speed,
        invisble,
        passthrough,
        bigbomb
    }
    public GameManager manager;
    public allpowerusp powerups;
    public float health;
    public float damgeradious;
    public void OnTriggerEnter(Collider other)
    {


        Entity enity = other.GetComponent<Entity>();
        if (powerups == allpowerusp.shild)
        {

            if (enity)
            {
                enity.StartCoroutine(enity.shildAcitavte());
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
         


        }else if(powerups == allpowerusp.speed)
        {
            if (enity)
            {
                enity.StartCoroutine(enity.SpeedBost());
                this.gameObject.SetActive(false);
            }
        }else if(powerups == allpowerusp.passthrough)
        {
            if (enity)
            {
                enity.StartCoroutine(enity.Invisible());
                this.gameObject.SetActive(false);
            }
            //need to pass the obstable
        }
        else if(powerups == allpowerusp.invisble) 
        {

            if (enity)
            {
                enity.StartCoroutine(enity.Hide());
                this.gameObject.SetActive(false);
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
                manager.botAll[i].ReduceHeath(100,null);
            }


        }
        


    }
}


