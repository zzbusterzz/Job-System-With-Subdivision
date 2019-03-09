using UnityEngine;

public class SelectionScript : MonoBehaviour
{
    public LayerMask layerToSelect;

    private RaycastHit hitinfo;
    private Ray ray;

    private Subdivide subdivisionInstance;
    // Start is called before the first frame update
    void Start()
    {
        subdivisionInstance = new Subdivide();
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction* 100);
        if (Physics.Raycast(ray, out hitinfo, 100, layerToSelect))
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if(hitinfo.collider != null)
                {
                    subdivisionInstance.SetSubdivisionModel(hitinfo.collider.gameObject);
                }
            }
        }
    }

    public void StartSubdivision()
    {
        subdivisionInstance.SubdivideMesh(true);
    }
}