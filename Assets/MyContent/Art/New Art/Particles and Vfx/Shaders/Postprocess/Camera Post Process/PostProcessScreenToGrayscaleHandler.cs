using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessScreenToGrayscaleHandler : MonoBehaviour
{
 //    public Material screenToGrayscaleMaterial;
 //    public float speed;
 //    public float grayAmount;
 //    bool _onTransition;
 //    bool _moreGray;
 //    float _black;
 //    Cleric _cleric;
 //    bool _onWait;
 //    public bool onWin;
 //
 //    private void Start()
 //    {
 //        LevelManager.Instance.Ghost.OnDeath += GhostDied;
 //        LevelManager.Instance.Ghost.OnLifeChange += (x) =>
 //         {
 //             if (_onTransition)
 //                 return;
 //             grayAmount = Mathf.Clamp(1f - x,0f,3f);
 //         };
 //        _cleric = FindObjectOfType<Cleric>();
 //        if (_cleric)
 //            _cleric.OnDeath += () => onWin = true;
 //    }
 //
 //    void Update ()
 //    {
 //        if (_onWait)
 //            return;
 //        if (onWin && LoadCreditsOnWin.Instance)
 //        {
 //            _black += Time.deltaTime * speed;
 //            if (_black >= 1f)
 //            {
 //                _black = Mathf.Clamp(_black, 0f, 1f);
 //                _onWait = true;
 //                LoadCreditsOnWin.Instance.AllowScene();
 //            }
 //        }
 //        if (_onTransition)
 //        {
 //            if (_moreGray)
 //            {
 //                grayAmount += Time.deltaTime*speed;
 //                if (grayAmount >= 1f)
 //                {
 //                    grayAmount = Mathf.Clamp(grayAmount, 0f, 1f);
 //                    _black += Time.deltaTime*speed;
 //                    if (_black >= 1f)
 //                    {
 //                        _black = Mathf.Clamp(_black, 0f, 1f);
 //                        _onWait = true;
 //                        LevelManager.Instance.Ghost.Death();
 //                        StartCoroutine(SeeAgain());
 //                    }
 //                }
 //            }
 //            else
 //            {
 //                if (_black > 0f)
 //                {
 //                    _black -= Time.deltaTime * speed;
 //                    _black = Mathf.Clamp(_black, 0f, 1f);
 //                }
 //                else
 //                {
 //                    grayAmount -= Time.deltaTime*speed;
 //                    if (grayAmount <= 0f)
 //                    {
 //                        grayAmount = Mathf.Clamp(grayAmount, 0f, 3f);
 //                        _moreGray = true;
 //                        _onTransition = false;
 //                        if (_cleric)
 //                            _cleric.ghostRespawning = false;
 //                    }
 //                }
 //            }
 //        }
 //        screenToGrayscaleMaterial.SetFloat("_GrayAmount", grayAmount);
 //        screenToGrayscaleMaterial.SetFloat("_BlackAmount", _black);
	// }
 //
 //    private void OnRenderImage(RenderTexture source, RenderTexture destination)
 //    {
 //        Graphics.Blit(source, destination, screenToGrayscaleMaterial);
 //    }
 //
 //    public void GhostDied()
 //    {
 //        _black = 0f;
 //        _onTransition = true;
 //        _moreGray = true;
 //        if (_cleric)
 //            _cleric.ghostRespawning = true;
 //    }
 //
 //    IEnumerator SeeAgain()
 //    {
 //        yield return new WaitForSeconds(1f);
 //        grayAmount = 0f;
 //        _moreGray = false;
 //        _onWait = false;
 //        LevelManager.Instance.Ghost.disabled = false;
 //    }
    
}
