using UnityEngine;

public class Powerups : MonoBehaviour
{
    public enum allpowerusp
    {
        shild,
        bomb,
    }

    public allpowerusp powerups;
    public float health;
    public void OnTriggerEnter(Collider other)
    {
        if (powerups == allpowerusp.shild)
        {
            if (other.GetComponent<Bot_Manager>())
            {

                other.GetComponent<Player_Manager>().StartCoroutine(other.GetComponent<Bot_Manager>().shildAcitavte());
                this.gameObject.SetActive(false);
            }
            else if(other.GetComponent<Player_Manager>())
            {
                other.GetComponent<Player_Manager>().StartCoroutine(other.GetComponent<Player_Manager>().shildAcitavte());
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
        effectobject.SetActive(true);

        


    }
}


