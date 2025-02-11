using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] allgrass;
    [SerializeField] private Color InsideGrass,outsidegrass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GetingintheGrass()
    {
        for (int i = 0; i < allgrass.Length; i++)
        {
            allgrass[i].material.color = InsideGrass;
        }
    }

    public void OutsideGrass()
    {
        for (int i = 0; i < allgrass.Length; i++)
        {
            allgrass[i].material.color = outsidegrass;
        }
    }
}
