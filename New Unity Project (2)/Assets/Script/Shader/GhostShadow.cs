using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostShadow : MonoBehaviour
{
    public static GhostShadow ghostShadow;
    //持續時間
    public float duration = 5f;
    //殘影間隔
    public float interval = 0.1f;


    
    SkinnedMeshRenderer[] meshRender;

    //殘影造型
    Shader ghostShader;
    private void Awake()
    {
        ghostShadow = this;
    }
    void Start()
    {       

        meshRender = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        ghostShader = Shader.Find("Xray");
        
    }

    private float lastTime = 0;

    private Vector3 lastPos = Vector3.zero;

    void Update()
    {
        //人物有位移才创建残影
        if (lastPos == this.transform.position)
        {
            return;
        }
        lastPos = this.transform.position;
        if (Time.time - lastTime < interval)
        {//残影间隔时间
            return;
        }
        lastTime = Time.time;

        if (meshRender == null)
            return;
        for (int i = 0; i < meshRender.Length; i++)
        {
            Mesh mesh = new Mesh();
            meshRender[i].BakeMesh(mesh);

            GameObject go = new GameObject();
            go.hideFlags = HideFlags.HideAndDontSave;

            GhostItem item = go.AddComponent<GhostItem>();//控制残影消失
            item.duration = duration;
            item.deleteTime = Time.time + duration;

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer meshRen = go.AddComponent<MeshRenderer>();
         
            meshRen.material = meshRender[i].material;
         
            meshRen.material.shader = ghostShader;//设置xray效果
            

            go.transform.localScale = meshRender[i].transform.localScale;
            go.transform.position = meshRender[i].transform.position;
            go.transform.rotation = meshRender[i].transform.rotation;

            item.meshRenderer = meshRen;
        }
    }
}