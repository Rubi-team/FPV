using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TriangleCounter : MonoBehaviour
{
    private class Counter
    {
        public string Name;
        public long TotalTris;
        public long DuplicateCount;

       //Ptet ça compte mieux 
        public float TrisPerDuplicate => DuplicateCount == 0 ? 0 : (float)TotalTris / DuplicateCount;
    }

    [SerializeField] private bool includeDuplicateMeshes = true;
    [SerializeField] private bool includeSkinnedMeshes = true;
    [SerializeField] private int topMeshCount = 10;

    [ContextMenu("Count Mesh Triangles")]
    private void Start()
    {
        var sceneCounts = new Dictionary<string, long>();
        var duplicateCounts = new Dictionary<Mesh, Counter>();
        var countedMeshes = new HashSet<Mesh>();
        long totalTris = 0;

        // MeshFilters
        var meshFilters = FindObjectsByType<MeshFilter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var mf in meshFilters)
        {
            var mesh = mf.sharedMesh;
            if (mesh == null) continue;

            var isDuplicate = countedMeshes.Contains(mesh);
            if (!includeDuplicateMeshes && isDuplicate) continue;

            if (!isDuplicate) countedMeshes.Add(mesh);

            var tris = CountMeshTriangles(mesh);

            var sceneName = mf.gameObject.scene.name;
            if (!sceneCounts.ContainsKey(sceneName)) sceneCounts[sceneName] = 0;
            sceneCounts[sceneName] += tris;
            totalTris += tris;

            if (!duplicateCounts.TryGetValue(mesh, out var counter))
            {
                counter = new Counter { Name = mesh.name, TotalTris = 0, DuplicateCount = 0 };
                duplicateCounts[mesh] = counter;
            }

            counter.TotalTris += tris;
            counter.DuplicateCount++;
        }

        // SkinnedMeshRenderers
        if (includeSkinnedMeshes)
        {
            var skinnedRenderers =
                FindObjectsByType<SkinnedMeshRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var smr in skinnedRenderers)
            {
                var mesh = smr.sharedMesh;
                if (mesh == null) continue;

                var isDuplicate = countedMeshes.Contains(mesh);
                if (!includeDuplicateMeshes && isDuplicate) continue;

                if (!isDuplicate) countedMeshes.Add(mesh);

                var tris = CountMeshTriangles(mesh);

                var sceneName = smr.gameObject.scene.name;
                if (!sceneCounts.ContainsKey(sceneName)) sceneCounts[sceneName] = 0;
                sceneCounts[sceneName] += tris;
                totalTris += tris;

                if (!duplicateCounts.TryGetValue(mesh, out var counter))
                {
                    counter = new Counter { Name = mesh.name, TotalTris = 0, DuplicateCount = 0 };
                    duplicateCounts[mesh] = counter;
                }

                counter.TotalTris += tris;
                counter.DuplicateCount++;
            }
        }

        // Top meshes by worst tris per duplicate ratio
        var topMeshes = duplicateCounts.Values
            .OrderByDescending(c => c.TrisPerDuplicate)
            .Take(topMeshCount);

        
        var sb = new StringBuilder();
        sb.AppendLine("Top meshes by worst tris/duplicate ratio:"); 
        var rank = 1;
        foreach (var c in topMeshes)
            sb.AppendLine(
                $"{rank++}-{c.Name}: {c.TotalTris} tris, {c.DuplicateCount} duplicates ({c.TrisPerDuplicate:F2} tris/instance)");
        Debug.Log(sb.ToString());

        Debug.Log($"Total: {totalTris} tris");
        foreach (var kvp in sceneCounts)
            Debug.Log($"{kvp.Key}: {kvp.Value} tris");
    }

    private long CountMeshTriangles(Mesh mesh)
    {
        long count = 0;
        for (var i = 0; i < mesh.subMeshCount; i++) count += (long)mesh.GetIndexCount(i) / 3;
        return count;
    }
}