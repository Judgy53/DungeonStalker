using UnityEngine;
using System.Collections;
 
[ExecuteInEditMode]
public class Overlay : MonoBehaviour
{
    public string shaderSource = "";

	private Material material;
 
	private void Awake ()
	{
		material = new Material(Shader.Find(shaderSource));
	}
	
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}
}