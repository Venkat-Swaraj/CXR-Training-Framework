using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShaderKeywordAdjuster: MonoBehaviour
{
    [SerializeField] private List<string> _shaderKeywords;
    [SerializeField] private List<Material> _materials;

    [ContextMenu(nameof(ToggleAllKeywords))]
    public void ToggleAllKeywords()
    {
        var firstMaterial = _materials[0];
        foreach (var keywordName in _shaderKeywords)
        {
            var isKeywordEnabledForFirstMaterial = firstMaterial.IsKeywordEnabled(keywordName); //keep materials consistent, always set all of them to first

            foreach (var material in _materials)
            {
                if (isKeywordEnabledForFirstMaterial)
                {
                    material.DisableKeyword(keywordName);
                }
                else
                {
                    material.EnableKeyword(keywordName);
                }
            }
        }
    }

    public List<bool> ResolveKeywordEnabledState()
    {
        var firstMaterial = _materials[0];
        return _shaderKeywords.Select(keywordName => firstMaterial.IsKeywordEnabled(keywordName)).ToList();
    }
}