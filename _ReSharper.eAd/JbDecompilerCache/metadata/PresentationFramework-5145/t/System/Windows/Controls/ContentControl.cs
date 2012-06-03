// Type: System.Windows.Controls.ContentControl
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using MS.Internal.PresentationFramework;
using System.Collections;
using System.ComponentModel;
using System.Runtime;
using System.Windows;
using System.Windows.Markup;

namespace System.Windows.Controls
{
    [ContentProperty("Content")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [DefaultProperty("Content")]
    public class ContentControl : Control, IAddChild
    {
        [CommonDependencyProperty] public static readonly DependencyProperty ContentProperty;
        [CommonDependencyProperty] public static readonly DependencyProperty HasContentProperty;
        [CommonDependencyProperty] public static readonly DependencyProperty ContentTemplateProperty;
        [CommonDependencyProperty] public static readonly DependencyProperty ContentTemplateSelectorProperty;
        [CommonDependencyProperty] public static readonly DependencyProperty ContentStringFormatProperty;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public ContentControl();

        protected internal override IEnumerator LogicalChildren { get; }

        [Bindable(true)]
        [CustomCategory("Content")]
        public object Content { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool HasContent { get; }

        [Bindable(true)]
        [CustomCategory("Content")]
        public DataTemplate ContentTemplate { get; set; }

        [CustomCategory("Content")]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplateSelector ContentTemplateSelector { get; set; }

        [Bindable(true)]
        [CustomCategory("Content")]
        public string ContentStringFormat { get; set; }

        #region IAddChild Members

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void IAddChild.AddChild(object value);

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void IAddChild.AddText(string text);

        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool ShouldSerializeContent();

        protected virtual void AddChild(object value);

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected virtual void AddText(string text);

        protected virtual void OnContentChanged(object oldContent, object newContent);
        protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate);
        protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector);
        protected virtual void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat);
    }
}
