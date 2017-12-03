using UnityEngine;
using System.Collections;

public class OutlineTest : MonoBehaviour {
	
	private SpriteRenderer m_sr;
	private MaterialPropertyBlock m_block ;

	// Use this for initialization
	void Start () {
		m_sr = GetComponent<SpriteRenderer>();
		m_block = new MaterialPropertyBlock();
	}
	
	public void OnChange(UnityEngine.UI.Slider slider){
		m_sr.GetPropertyBlock(m_block);
		m_block.SetFloat("_Outline",slider.value);
		m_sr.SetPropertyBlock(m_block);
	}
}
