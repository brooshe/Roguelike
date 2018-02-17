using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Sprites;

namespace UI
{
    /// <summary>
    /// Image is a textured element in the UI hierarchy.
    /// </summary>

    [AddComponentMenu("UI/MinimapImage")]
    public class MinimapImage : MaskableGraphic
    {

        static protected Material s_ETC1DefaultUI = null;

        [FormerlySerializedAs("m_Frame")]
        [SerializeField] private Sprite m_Sprite;
        public Sprite sprite { get { return m_Sprite; } set { m_Sprite = value; SetAllDirty(); } }

        [NonSerialized]
        private Sprite m_OverrideSprite;
        public Sprite overrideSprite { get { return activeSprite; } set { m_OverrideSprite = value; SetAllDirty(); } }

        private Sprite activeSprite { get { return m_OverrideSprite != null ? m_OverrideSprite : sprite; } }

        [SerializeField] private bool m_PreserveAspect = false;
        public bool preserveAspect { get { return m_PreserveAspect; } set { m_PreserveAspect = value; SetVerticesDirty(); } }

        [SerializeField] private bool m_FillCenter = true;
        public bool fillCenter { get { return m_FillCenter; } set { m_FillCenter = value; SetVerticesDirty(); } }

        /// Amount of the Image shown. 0-1 range with 0 being nothing shown, and 1 being the full Image.
        [Range(0, 1)]
        [SerializeField] private float m_FillAmount = 1.0f;
        public float fillAmount { get { return m_FillAmount; } set { m_FillAmount = Mathf.Clamp01(value); SetVerticesDirty(); } }

        /// Whether the Image should be filled clockwise (true) or counter-clockwise (false).
        [SerializeField] private bool m_FillClockwise = true;
        public bool fillClockwise { get { return m_FillClockwise; } set { m_FillClockwise = value; SetVerticesDirty(); } }

        /// Controls the origin point of the Fill process. Value means different things with each fill method.
        [SerializeField] private int m_FillOrigin;
        public int fillOrigin { get { return m_FillOrigin; } set { m_FillOrigin = value; SetVerticesDirty(); } }

        // Not serialized until we support read-enabled sprites better.
        private float m_AlphaHitTestMinimumThreshold = 0;

        [Obsolete("eventAlphaThreshold has been deprecated. Use eventMinimumAlphaThreshold instead (UnityUpgradable) -> alphaHitTestMinimumThreshold")]
        public float eventAlphaThreshold { get { return 1 - alphaHitTestMinimumThreshold; } set { alphaHitTestMinimumThreshold = 1 - value; } }
        public float alphaHitTestMinimumThreshold { get { return m_AlphaHitTestMinimumThreshold; } set { m_AlphaHitTestMinimumThreshold = value; } }

        protected MinimapImage()
        {
            useLegacyMeshGeneration = false;
        }

        /// <summary>
        /// Default material used to draw everything if no explicit material was specified.
        /// </summary>

        static public Material defaultETC1GraphicMaterial
        {
            get
            {
                if (s_ETC1DefaultUI == null)
                    s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
                return s_ETC1DefaultUI;
            }
        }
        public Texture2D mainTex;
        /// <summary>
        /// Image's texture comes from the UnityEngine.Image.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (mainTex == null)
                    return s_WhiteTexture;
                return mainTex;

                //if (activeSprite == null)
                //{
                //    if (material != null && material.mainTexture != null)
                //    {
                //        return material.mainTexture;
                //    }
                //    return s_WhiteTexture;
                //}

                //return activeSprite.texture;
            }
        }

        /// <summary>
        /// Whether the Image has a border to work with.
        /// </summary>

        public bool hasBorder
        {
            get
            {
                if (activeSprite != null)
                {
                    Vector4 v = activeSprite.border;
                    return v.sqrMagnitude > 0f;
                }
                return false;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (activeSprite)
                    spritePixelsPerUnit = activeSprite.pixelsPerUnit;

                float referencePixelsPerUnit = 100;
                if (canvas)
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;

                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }

        public override Material material
        {
            get
            {
                if (m_Material != null)
                    return m_Material;

                if (activeSprite && activeSprite.associatedAlphaSplitTexture != null)
                    return defaultETC1GraphicMaterial;

                return defaultMaterial;
            }

            set
            {
                base.material = value;
            }
        }

        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = activeSprite == null ? Vector4.zero : DataUtility.GetPadding(activeSprite);
            var size = activeSprite == null ? Vector2.zero : new Vector2(activeSprite.rect.width, activeSprite.rect.height);

            Rect r = GetPixelAdjustedRect();
            Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

        public override void SetNativeSize()
        {
            if (activeSprite != null)
            {
                float w = activeSprite.rect.width / pixelsPerUnit;
                float h = activeSprite.rect.height / pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        protected override void UpdateGeometry()
        {
            //DoMeshGeneration();
        }

        List<Vector3> positions = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();


        public void ClearQuad()
        {
            positions.Clear();
            uvs.Clear();
            triangles.Clear();
        }
        public void AddQuad(Rect pos, Rect uv)
        {
            int count = positions.Count;
            positions.Add(new Vector3(pos.xMin, pos.yMin));
            positions.Add(new Vector3(pos.xMax, pos.yMin));
            positions.Add(new Vector3(pos.xMax, pos.yMax));
            positions.Add(new Vector3(pos.xMin, pos.yMax));

            uvs.Add(uv.min);
            uvs.Add(new Vector2(uv.xMax, uv.yMin));
            uvs.Add(uv.max);
            uvs.Add(new Vector2(uv.xMin, uv.yMax));

            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            triangles.Add(count);
            triangles.Add(count + 2);
            triangles.Add(count + 3);
        }
        public void SetMeshDirty()
        {
            workerMesh.Clear();
            workerMesh.SetVertices(positions);
            workerMesh.SetUVs(0, uvs);
            workerMesh.SetTriangles(triangles, 0);
            workerMesh.colors = null;
            canvasRenderer.SetMesh(workerMesh);
        }


        private void DoMeshGeneration()
        {
            ClearQuad();
            if (rectTransform != null && rectTransform.rect.width >= 0 && rectTransform.rect.height >= 0)
            {
                Rect r = GetPixelAdjustedRect();
                Debug.LogFormat("minimap rect:{0}", r);

                AddQuad(r, new Rect(0,0,1,1));

            }

            //    OnPopulateMesh(s_VertexHelper);
            //else
            //    s_VertexHelper.Clear(); // clear the vertex helper so invalid graphics dont draw.

            //var components = ListPool<Component>.Get();
            //GetComponents(typeof(IMeshModifier), components);

            //for (var i = 0; i < components.Count; i++)
            //    ((IMeshModifier)components[i]).ModifyMesh(s_VertexHelper);

            //ListPool<Component>.Release(components);

            //s_VertexHelper.FillMesh(workerMesh);
            SetMeshDirty();
        }

        /// <summary>
        /// Update the UI renderer mesh.
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (activeSprite == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }


            GenerateSimpleSprite(toFill, m_PreserveAspect);

        }

        /// <summary>
        /// Update the renderer's material.
        /// </summary>

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            // check if this sprite has an associated alpha texture (generated when splitting RGBA = RGB + A as two textures without alpha)

            if (activeSprite == null)
            {
                canvasRenderer.SetAlphaTexture(null);
                return;
            }

            Texture2D alphaTex = activeSprite.associatedAlphaSplitTexture;

            if (alphaTex != null)
            {
                canvasRenderer.SetAlphaTexture(alphaTex);
            }
        }

        #region Various fill functions
        /// <summary>
        /// Generate vertices for a simple Image.
        /// </summary>
        void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (activeSprite != null) ? DataUtility.GetOuterUV(activeSprite) : Vector4.zero;

            var color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));
            Debug.LogFormat("v:{0} {1} {2} {3}", v.x, v.y, v.z, v.w );
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }


        static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
        {
            int startIndex = vertexHelper.currentVertCount;

            for (int i = 0; i < 4; ++i)
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        #endregion

        public virtual void CalculateLayoutInputHorizontal() {}
        public virtual void CalculateLayoutInputVertical() {}

        public virtual int layoutPriority { get { return 0; } }

 
    }
}
