using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVOutline : MonoBehaviour {

	[HideInInspector]
    public float outlineActive = 0; // value between 0 and 1

    [Tooltip("The thickness of the outline effect")]
    public float outlineThickness = 1f;
    public Color outlineColor;

    public Material outlineMaterial;

    private GameObject outlineModel;
    private Material outlineModelMaterial;

    private float lastIsActive = 100;

	// Use this for initialization
	void Start () {
        this.RefreshHighlightMesh();
	}
	
	// Update is called once per frame
	void Update () {
        if (this.lastIsActive != this.outlineActive) {
            outlineModelMaterial.SetFloat("_Alpha", this.outlineActive);
            outlineModelMaterial.SetFloat("_Thickness", this.outlineThickness);
            outlineModelMaterial.SetColor("_OutlineColor", (Color)this.outlineColor);
            this.lastIsActive = this.outlineActive;

            if (this.outlineActive > 0) {
                this.outlineModel.SetActive(true);
            } else {
                this.outlineModel.SetActive(false);
            }
        }
    }

    public void RefreshHighlightMesh() {
        if (this.outlineModel != null) {
            Destroy(outlineModel);
        }

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = gameObject.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;

			i++;
		}


		this.outlineModel = new GameObject(name + "OutlineModel");
		outlineModel.transform.SetParent(this.gameObject.transform, false);

		MeshFilter filter = outlineModel.AddComponent<MeshFilter>();
		filter.mesh = new Mesh ();
		filter.mesh.CombineMeshes (combine);

		MeshRenderer renderer = outlineModel.AddComponent<MeshRenderer>();
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		renderer.receiveShadows = false;
		renderer.material = outlineMaterial;
		this.outlineModelMaterial = renderer.material;

		this.outlineModel.SetActive(false);
    }
}
