using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace MKW.GUI.Controls
{
    internal sealed class SelectorBoxItemAutomationPeer
        : SelectorItemAutomationPeer
        , IVirtualizedItemProvider
        , ISelectionItemProvider
    {
        public SelectorBoxItemAutomationPeer(object item, SelectorBoxAutomationPeer selectorBoxAutomationPeer)
            : base(item, selectorBoxAutomationPeer)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return nameof(SelectorBoxItem);
        }
    }
}
