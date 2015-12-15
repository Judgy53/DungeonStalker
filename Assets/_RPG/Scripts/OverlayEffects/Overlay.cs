using UnityEngine;
using System.Collections;
 
[ExecuteInEditMode]
public class Overlay : MonoBehaviour
{
	public Material material;
 
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}
}