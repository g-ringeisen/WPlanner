using System;
using System.Collections.Generic;
using System.Text;

namespace WurthPlanner.Models;

public class WorkItemDescription
{
    private string  _content = string.Empty;
    private string? _excerpt;
    
    public string Text { 
        get => _content;
        set {
            _content = value;
            _excerpt = null;
        }
    }
    public string MimeType { get; set; } = "text/plain";
    public string? Excerpt
    {
        get
        {
            if (_excerpt == null && MimeType != null && MimeType.StartsWith("text/") && !string.IsNullOrEmpty(_content))
            {
                // Supprimer les balises HTML
                var noHtml = System.Text.RegularExpressions.Regex.Replace(_content, "<.*?>", string.Empty);
                // Limiter à 50 caractères
                _excerpt = noHtml.Length > 30 ? noHtml.Substring(0, 27) + "..." : noHtml;
            }
            return _excerpt;
        }

        private set => _excerpt = value;
    }
}
