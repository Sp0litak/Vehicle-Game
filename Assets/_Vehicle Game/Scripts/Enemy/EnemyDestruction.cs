using InstantDestruction;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyDestruction : InstantDestructionSimple
{
    public void SetMesh(SkinnedMeshRenderer source)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Enemy death pose";
        source.BakeMesh(mesh, false);

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterials = source.sharedMaterials;
        transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
        transform.localScale = source.transform.lossyScale;
    }

    public void SetMesh(MeshFilter source)
    {
        GetComponent<MeshFilter>().sharedMesh = Instantiate(source.sharedMesh);
        GetComponent<MeshRenderer>().sharedMaterials = source.GetComponent<MeshRenderer>().sharedMaterials;
        transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
        transform.localScale = source.transform.lossyScale;
    }

    protected override void Start()
    {
        particleGradient = new Gradient();
        base.Start();
        audioSource.mute = true;
        OnTrigger();
    }

    protected override void OnDestroy()
    {
        if (destructionParticleSystem != null)
            Destroy(destructionParticleSystem.gameObject);

        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh != null)
            Destroy(filter.sharedMesh);

        base.OnDestroy();
    }
}
