using System.Collections.Generic;
using UnityEngine;

public class Subdivide
{  
    //var material : Material;
    bool buildmesh = false;
    private Vector3[] verts;
    private Vector3[] norms;
    private Vector2[] uvs;
    private int[] trigs;
    private Mesh mesh;
    private Mesh originalMesh;

    private GameObject obejctToModify;

    //var subdivision : boolean = false;
    //var useobjectsmaterial : boolean = false;
    //var forceaddmeshcollider : boolean = false;
    //var sides : boolean = false;
    //var middle : boolean = false;

    Queue<Vector3> nv;
    Queue<Vector3> nu;
    Queue<Vector3> nn;
    List<int> nt;

    void Update()
    {
        //if (Input.GetKeyDown("1"))
        //    SubdivideMesh(false);
        //if (Input.GetKeyDown("2"))
        //    SubdivideMesh(true);
        //if (Input.GetKeyDown("x"))
        //{
        //    CopyMesh(originalMesh, mesh);
        //}
    }

    public Subdivide()
    {
        ///////////////////////////////////////////////////////////
        //// initialize
        ///////////////////////////////////////////////////////////
        nv = new Queue<Vector3>();
        nu = new Queue<Vector3>();
        nn = new Queue<Vector3>();
        nt = new List<int>();
    }

    public void SetSubdivisionModel(GameObject model)
    {
        if(obejctToModify == model)
        {
            obejctToModify = null;
        }
        else
        {
            obejctToModify = model;
        }

        if (obejctToModify.GetComponent<MeshFilter>() == null)
        {
            buildmesh = true;
            obejctToModify.AddComponent<MeshFilter>();
        }

        if (obejctToModify.GetComponent<MeshRenderer>() == null)
            obejctToModify.AddComponent<MeshRenderer>();
        
        if(obejctToModify.GetComponent<MeshCollider>() == null)
            obejctToModify.AddComponent<MeshCollider>();        

        updatemesh();

        //if(!useobjectsmaterial)
        //	GetComponent(MeshRenderer).material = material;

        ///////////////////////////////////////////////////////////
        //// build mesh
        ///////////////////////////////////////////////////////////

        if (buildmesh)
        {
            //// vertices
            verts = new[] { new Vector3(0, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(0, 1, -1) };

            //// uvs
            uvs = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0) };

            //// triangles
            trigs = new[] { 0, 1, 2, 1, 3, 2 };

            applymesh();
            mesh.RecalculateNormals();

            Debug.Log(verts.Length);
        }

        originalMesh = new Mesh();
        CopyMesh(mesh, originalMesh);
    }

    public void SubdivideMesh(bool center)
    {
        if(obejctToModify == null)
        {
            Debug.Log("Not selected anything");
            return;
        }

        verts = mesh.vertices;
        trigs = mesh.triangles;
        uvs = mesh.uv;
        norms = mesh.normals;

        nt.Clear();

        for (int i = 0; i < verts.Length; i++)
        {
            nv.Enqueue(verts[i]);
        }

        for (int i = 0; i < norms.Length; i++)
        {
             nn.Enqueue(norms[i]);
        }

        
        for (int i = 0; i < uvs.Length; i++)
        {
            nu.Enqueue(uvs[i]);
        }

        for (int i = 0; i < trigs.Length; i++)
        {
            nt.Add(trigs[i]);
        }


        Debug.Log("enter subdividing: " + verts.Length);

        if (!center)
        {
            for (int i = 2; i < trigs.Length; i += 3) {

                int p0trigwho = trigs[i - 2];
                int p1trigwho = trigs[i - 1];
                int p2trigwho = trigs[i];

                int p0trigwhere = i - 2;
                int p1trigwhere = i - 1;
                int p2trigwhere = i;

                Vector3 p0  = verts[p0trigwho];
                Vector3 p1 = verts[p1trigwho];
                Vector3 p2  = verts[p2trigwho];

                Vector2 pn0  = norms[p0trigwho];
                Vector2 pn1  = norms[p1trigwho];
                Vector2 pn2  = norms[p2trigwho];

                Vector2 pu0  = uvs[p0trigwho];
                Vector2 pu1  = uvs[p1trigwho];
                Vector2 pu2  = uvs[p2trigwho];

                Vector3 p0mod  = (p0 + p1) / 2;
                Vector3 p1mod  = (p1 + p2) / 2;
                Vector3 p2mod  = (p0 + p2) / 2;

                Vector3 pn0mod  = ((pn0 + pn1) / 2).normalized;
                Vector3 pn1mod  = ((pn1 + pn2) / 2).normalized;
                Vector3 pn2mod  = ((pn0 + pn2) / 2).normalized;

                Vector2 pu0mod  = (pu0 + pu1) / 2;
                Vector2 pu1mod  = (pu1 + pu2) / 2;
                Vector2 pu2mod  = (pu0 + pu2) / 2;

                var p0modtrigwho = nv.Count;
                var p1modtrigwho = nv.Count + 1;
                var p2modtrigwho = nv.Count + 2;

                nv.Enqueue(p0mod);
                nv.Enqueue(p1mod);
                nv.Enqueue(p2mod);

                nn.Enqueue(pn0mod);
                nn.Enqueue(pn1mod);
                nn.Enqueue(pn2mod);

                nu.Enqueue(pu0mod);
                nu.Enqueue(pu1mod);
                nu.Enqueue(pu2mod);

                nt[p0trigwhere] = p0trigwho;
                nt[p1trigwhere] = p0modtrigwho;
                nt[p2trigwhere] = p2modtrigwho;

                nt.Add(p0modtrigwho);
                nt.Add(p1modtrigwho);
                nt.Add(p2modtrigwho);

                nt.Add(p0modtrigwho);
                nt.Add(p1trigwho);
                nt.Add(p1modtrigwho);

                nt.Add(p2modtrigwho);
                nt.Add(p1modtrigwho);
                nt.Add(p2trigwho);
            }
        }
        else
        {
            for (int ii = 2; ii < trigs.Length; ii += 3)
            {
                int p0trigwhomi  = trigs[ii - 2];
                int p1trigwhomi  = trigs[ii - 1];
                int p2trigwhomi  = trigs[ii];

                int p0trigwheremi  = ii - 2;
                int p1trigwheremi  = ii - 1;
                int p2trigwheremi  = ii;

                Vector3 p0mi  = verts[p0trigwhomi];
                Vector3 p1mi  = verts[p1trigwhomi];
                Vector3 p2mi  = verts[p2trigwhomi];

                Vector3 p0mn  = norms[p0trigwhomi];
                Vector3 p1mn  = norms[p1trigwhomi];
                Vector3 p2mn  = norms[p2trigwhomi];

                Vector2 p0mu  = uvs[p0trigwhomi];
                Vector2 p1mu  = uvs[p1trigwhomi];
                Vector2 p2mu  = uvs[p2trigwhomi];

                Vector3 p0modmi  = (p0mi + p1mi + p2mi) / 3;
                Vector3 p0modmn  = ((p0mn + p1mn + p2mn) / 3).normalized;
                Vector2 p0modmu  = (p0mu + p1mu + p2mu) / 3;

                var p0modtrigwhomi = nv.Count;

                nv.Enqueue(p0modmi);
                nn.Enqueue(p0modmn);
                nu.Enqueue(p0modmu);

                nt[p0trigwheremi] = p0trigwhomi;
                nt[p1trigwheremi] = p1trigwhomi;
                nt[p2trigwheremi] = p0modtrigwhomi;

                nt.Add(p0modtrigwhomi);
                nt.Add(p1trigwhomi);
                nt.Add(p2trigwhomi);

                nt.Add(p0trigwhomi);
                nt.Add(p0modtrigwhomi);
                nt.Add(p2trigwhomi);
            }
        }

        verts = new Vector3[nv.Count];
        for(int i = 0; i < verts.Length; i++)
        {
            verts[i] = nv.Dequeue();
        }

        norms = new Vector3[nn.Count];
        for (int i = 0; i < norms.Length; i++)
        {
            norms[i] = nn.Dequeue();
        }

        uvs = new Vector2[nu.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = nu.Dequeue();
        }

        trigs = new int[nt.Count];
        for (int i = 0; i < trigs.Length; i++)
        {
            trigs[i] = nt[i];
        }

        //applyuvs();
        applymesh();
        //mesh.RecalculateNormals();

        Debug.Log("exit subdividing: " + verts.Length);
    }

    void applyuvs()
    {
        uvs = new Vector2[verts.Length];
        for (int i = 0; i < verts.Length; i++)
		uvs[i] = new Vector2(verts[i].x, verts[i].y);
    }

    void updatemesh()
    {
        //mesh = new Mesh();
        mesh = obejctToModify.GetComponent<MeshFilter>().mesh;
    }

    void applymesh()
    {
        Debug.Log(verts.Length);
        if (verts.Length > 65000)
        {
            Debug.Log("Exiting... Too many vertices");
            return;
        }
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        if (mesh.uv2 != null)
            mesh.uv2 = uvs;
        mesh.normals = norms;
        mesh.triangles = trigs;
        mesh.RecalculateBounds();
        if (obejctToModify.GetComponent<MeshCollider>() != null)
            obejctToModify.GetComponent<MeshCollider>().sharedMesh = mesh;
        updatemesh();
    }

    void CopyMesh(Mesh fromMesh, Mesh toMesh)
    {
        toMesh.Clear();
        toMesh.vertices = fromMesh.vertices;
        toMesh.normals = fromMesh.normals;
        toMesh.uv = fromMesh.uv;
        toMesh.triangles = fromMesh.triangles;
    }
}