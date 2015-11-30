using UnityEngine;
using System.Collections;
 
[ExecuteInEditMode]
public class Overlay : MonoBehaviour
{
    //public Shader shader = null;

	public Material material;
 
	/*private void Awake ()
	{
		material = new Material(shader);
	}*/
	
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}
}