using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public class WaveInfo
    {
        public enum WaveType
        {
            CIRCULAR,DIRECTIONAL
        }
        public Vector3 pos;
        public float length;
        public float speed;
        public float height;
        public float steepness;
        public float lifeTime;
        public WaveType type;


        public WaveInfo(Vector3 p, float l, float s,float h,float stps, float lt, WaveType t)
        {
            pos = p;
            length = l;
            speed = s;
            height = h;
            steepness = stps;
            lifeTime = lt;
            type = t;
        }
    }

    public Material material = null;
    public float Length = 5.0f;
    public float speed = 1.0f;
    public float _Ai = 20.0f;
    public float _Qi = 0.9f;
    public float _AlphaScale = 0.5f;
    public const int WaveWidth = 25;
    public const int WaveHeight = 25;
    public const int WaveWidthVertexCount = WaveWidth * 10;
    public const int WaveHeightVertexCount = WaveHeight * 10;
    // Use this for initialization

    protected List<WaveInfo> waveList;

    public Texture2D waveTexture = null;
    public List<float> waveArr = null;

    public Mesh CreateMesh(int edg_x, int edg_y)
    {

        int m = edg_y; //row  
        int n = edg_x;  //col  
        float width = WaveWidth;
        float height = WaveHeight;
        Vector3[] vertices = new Vector3[(m + 1) * (n + 1)];//the positions of vertices 
        Vector3[] normals = new Vector3[(m + 1) * (n + 1)];


        Vector2[] uv = new Vector2[(m + 1) * (n + 1)];
        int[] triangles = new int[6 * m * n];
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = i % (n + 1);
            float y = i / (n + 1);
            float x_pos = x / n * width - width / 2;
            float y_pos = y / m * height - height / 2;
            vertices[i] = new Vector3(x_pos,0 , y_pos);
            normals[i] = new Vector3(0, 1, 0);
            float u = x / edg_x;
            float v = y / edg_y;
            uv[i] = new Vector2(u, v);

        }

        int ii = 0;
        for(int y = 0; y < m; ++y)
        {
            for(int x = 0; x < n; ++x)
            {
                int[] triIndex = new int[3];

                triIndex[0] = x + y * (n + 1);
                triIndex[1] = triIndex[0] + 1;
                triIndex[2] = triIndex[0] + (n + 1);

                triangles[ii++] = triIndex[0];
                triangles[ii++] = triIndex[2];
                triangles[ii++] = triIndex[1];

                triIndex[0] = x + (y + 1) * (n + 1);
                triIndex[1] = x + y * (n + 1) + 1;
                triIndex[2] = triIndex[0] + 1;

                triangles[ii++] = triIndex[0];
                triangles[ii++] = triIndex[2];
                triangles[ii++] = triIndex[1];
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = normals;
        return mesh;
    }


    void GenerateWaveTexture()
    {
        waveTexture = new Texture2D(WaveWidthVertexCount + 1, WaveHeightVertexCount + 1, TextureFormat.RGBAFloat,false);
        waveTexture.wrapMode = TextureWrapMode.Clamp;
        for (int iz = 0; iz <= WaveHeightVertexCount; ++iz)
        {
            for (int ix = 0; ix <= WaveWidthVertexCount; ++ix)
            {
                Color r = waveTexture.GetPixel(ix, iz);
                r.r = 0;
                r.g = 0;
                r.b = 0;
                r.a = 0;
                waveTexture.SetPixel(ix, iz, r);
            }
        }
    }

    void ClearWaveTexture()
    {
        for (int iz = 0; iz <= WaveHeightVertexCount; ++iz)
        {
            for (int ix = 0; ix <= WaveWidthVertexCount; ++ix)
            {
                waveTexture.SetPixel(ix, iz, Color.black);
            }
        }
    }

    void UpdateWave()
    {
        ClearWaveTexture();
        for(int i = 0; i < waveList.Count; ++i )
        {
            WaveInfo wave = waveList[i];
            if (wave.type == WaveInfo.WaveType.CIRCULAR)
            {

            }
            else if(wave.type == WaveInfo.WaveType.DIRECTIONAL)
            {
                //for(int x = (int)(wave.pos.x - wave.length / 2);x <= (int)(wave.pos.x + wave.length / 2); ++x)
               
            for(int iz = 0; iz <= WaveHeightVertexCount; ++iz)
                {
                    for (int ix = 0; ix <= WaveWidthVertexCount; ++ix)
                    {
                        if (ix < 0 || ix > WaveWidthVertexCount)
                        {
                            continue;
                        }
                        float z = (float)iz / WaveHeightVertexCount * WaveHeight;
                        float x = (float)ix / WaveWidthVertexCount * WaveWidth;
                        float speed = wave.speed * Mathf.PI * 2.0f / wave.length;
                        float _Wt = Mathf.PI * 2.0f / wave.length;
                        float it = x * _Wt  + (Time.time - wave.lifeTime) * speed;
                        float y = wave.height * Mathf.Sin(it);
                        float qi = _Qi / (wave.length * _Ai);
                        float deltaX = qi * _Ai * x * Mathf.Cos(_Wt * x + (Time.time - wave.lifeTime) * speed);
                        int index = ix +iz * (WaveWidth + 1);

                        Color r = waveTexture.GetPixel(ix, iz);
                        r.r = deltaX;
                        r.g += y;
                        waveTexture.SetPixel(ix, iz, r);
                    }
                }
                waveTexture.Apply();
            }
        }
    }

    void Start () {
        waveList = new List<WaveInfo>();
        GenerateWaveTexture();
        Debug.Log("Start_1");
        if (material == null)
        {
            gameObject.GetComponent<MeshFilter>().mesh = CreateMesh(WaveWidthVertexCount, WaveHeightVertexCount);
            Renderer renderer = gameObject.GetComponent<Renderer>();
            MeshCollider collider = gameObject.GetComponent<MeshCollider>();
            if (renderer == null)
            {
                Debug.LogWarning("Cannot find a renderer.");
                return;
            }

            material = renderer.material;

            
            material.SetFloat("_W", Mathf.PI * 2.0f / Length);
            
            Debug.Log("Length " + Length);
            Debug.Log("_Speed " + speed * Mathf.PI * 2.0f / Length);
            Debug.Log("_Ai " + _Ai);
            material.SetFloat("_Speed", speed * Mathf.PI * 2.0f / Length );
            material.SetFloat("_Ai", _Ai);
            material.SetFloat("_AlphaScale", _AlphaScale);
            //material.SetInt("_Width", WaveWidth);

            Debug.Log("Start_2");

        }
        waveList.Add(new WaveInfo(new Vector3(), Length, speed, _Ai, _Qi, lasttime, WaveInfo.WaveType.DIRECTIONAL));
        lasttime = Time.time;
    }
    // Update is called once per frame
    float lasttime;
    void Update () {
        //if(Time.time - lasttime > 3)
        //{
        //    lasttime = Time.time;
        //    waveList.Add(new WaveInfo(new Vector3(), Length, speed, _Ai, _Qi, lasttime, WaveInfo.WaveType.DIRECTIONAL));
        //}
        UpdateWave();

        material.SetFloat("_W", Mathf.PI * 2.0f / Length);
        material.SetFloat("_Speed", speed * Mathf.PI * 2.0f / Length);
        material.SetFloat("_Ai", _Ai);
        material.SetFloat("_Time1", Time.time);
        material.SetFloat("_AlphaScale", _AlphaScale);
        material.SetFloat("_Qi", _Qi / (Length * _Ai));
        material.SetInt("_Width", WaveWidth);
        material.SetTexture("_WaveTex", waveTexture);
    }
}
