using System.Windows.Automation.Peers;

namespace MKW.GUI.Controls
{
    internal sealed class SelectorBoxAutomationPeer : SelectorAutomationPeer
    {
        public SelectorBoxAutomationPeer(SelectorBox owner)
            : base(owner)
        {
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new SelectorBoxItemAutomationPeer(item, this);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return nameof(SelectorBox);
        }
    }
}
